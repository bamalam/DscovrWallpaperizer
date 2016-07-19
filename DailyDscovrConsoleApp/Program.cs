using System;
using System.Linq;
using System.Device.Location;
using System.Net.Http;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DailyDscovrConsoleApp
{
    internal class Program
    {
        const string _dscovrApi = "http://epic.gsfc.nasa.gov/api/images.php";
        const string _dscovrImages = "http://epic.gsfc.nasa.gov/epic-archive/png/";
        private static void Main(string[] args)
        {
            try
            {
                var location = GetLocation();

                Console.WriteLine($"got location: {location}");

                var imageUri = GetLatestImageUri(location.Longitude);

                Console.WriteLine($"Latest image: {imageUri}");

                var image = GetImage(imageUri);

                Console.WriteLine("Succesfully Downloaded image");

                if (SetWallpaper(image))
                {
                    Console.WriteLine("Succesfully Set Wallpaper");
                }
                else
                {
                    Console.WriteLine("Setting Wallpaper failed");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public static GeoCoordinate GetLocation()
        {
            using (var watcher = new GeoCoordinateWatcher())
            {
                watcher.TryStart(false, TimeSpan.FromMilliseconds(5000));
                
                var coord = watcher.Position.Location;

                if (!coord.IsUnknown) return coord;

                Thread.Sleep(5000);

                // try one more time...
                watcher.TryStart(false, TimeSpan.FromMilliseconds(10000));

                coord = watcher.Position.Location;
                if (!coord.IsUnknown) return coord;
                // todo Could fall back to a web service
                Console.WriteLine("Could not acquire location");
                throw new Exception("Failed to acquire location...");
            }
        }

        public static bool SetWallpaper(Image wallpaper)
        {
            return Wallpaper.Set(wallpaper, Wallpaper.Style.Centered);
        }
        
        public static Uri GetLatestImageUri(double longitude)
        {
            if (longitude > 180 || longitude < -180)
                throw new ArgumentOutOfRangeException("Longitude must be between -180 and 180");
            // target +-20, being careful to wrap around the globe...
            var westTarget = longitude - 20 < -180 ? (longitude - 20) + 360 : longitude - 20;
            var eastTarget = longitude + 20 > 180 ? (longitude + 20) - 360 : longitude + 20;
            using (var client = new HttpClient())
            using (var result = client.GetAsync($"{_dscovrApi}?w={westTarget}&e={eastTarget}").Result)
            {
                if (!result.IsSuccessStatusCode)
                    throw new Exception($"Failed to retrieve metadata, Status code: {result.StatusCode}");
                var json = result.Content.ReadAsStringAsync().Result;

                var images = JsonConvert.DeserializeObject<List<ImageData>>(json);

                if (images == null)
                    throw new Exception("Invalid response");

                if (!images.Any())
                    throw new Exception("No Images found");

                // inefficient but shrug. Shouldn't be a big array to deal with
                var latestImage = images.OrderBy(image => image.date)
                                         .ThenBy(image => Math.Abs(image.coordinates.centroid_coordinates.lon - longitude)).First();

                // no idea what the time zone is... The website does not specify :/
                Console.WriteLine($"Latest image was taken at {latestImage.date}, aimed at {latestImage.coordinates.centroid_coordinates}");

                return new Uri($"{_dscovrImages}{latestImage.image}.png", UriKind.Absolute);
            }
        }

        public static Image GetImage(Uri imageUrl)
        {
            using (var client = new HttpClient())
            using (var result = client.GetAsync(imageUrl).Result)
            {
                if (!result.IsSuccessStatusCode)
                    throw new Exception($"Failed to retrieve image, Status code: {result.StatusCode}");

                using (var stream = result.Content.ReadAsStreamAsync().Result)
                    return Image.FromStream(stream);
            }
        }
    }
}

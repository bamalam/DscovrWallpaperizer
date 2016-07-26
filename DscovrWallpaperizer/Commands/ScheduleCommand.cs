using ManyConsole;
using Microsoft.Win32.TaskScheduler;
using System;

namespace DailyDscovrConsoleApp.Commands
{
    public class ScheduleCommand : ConsoleCommand
    {
        private static bool Install = true;
        private static short RunInterval = 1;
        public ScheduleCommand()
        {
            IsCommand("DailyDscovr", "Schedules the UpdateWallpaper command to automatically run");

            HasLongDescription("This either sets or removes the Set wallpaper command from the daily schedule.");

            HasOption("r|remove", "Removes the DSCOVR wallpaper task", _ => Install = false);

            HasOption("i|interval", "Sets the interval in days between wallpaper updates", i => RunInterval = Convert.ToInt16(i));
        }

        private static string GetCurrentDir()
        {
            var path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

            return System.IO.Path.GetDirectoryName(path);
        }

        public override int Run(string[] remainingArguments)
        {
            using (TaskService ts = new TaskService())
            {
                if (Install)
                {
                    Console.WriteLine("Installing task....");
                    var td = ts.NewTask();
                    td.RegistrationInfo.Description = "Updates wallpaper, with latest from Dscovr for the users current location";
                    td.Settings.RunOnlyIfIdle = true;
                    td.Settings.RunOnlyIfNetworkAvailable = true;
                    td.Settings.DisallowStartIfOnBatteries = true;
                    td.Settings.StartWhenAvailable = true;

                    // set it retry every 20 mins for an hour
                    td.Settings.RestartCount = 3;
                    td.Settings.RestartInterval = new TimeSpan(0, 20, 0);

                    td.Triggers.Add(new DailyTrigger { DaysInterval = RunInterval });

                    // todo remove hardcoded paths
                    td.Actions.Add(new ExecAction("DscovrWallpaperizer.exe", "UpdateWallpaper", AppDomain.CurrentDomain.BaseDirectory));

                    ts.RootFolder.RegisterTaskDefinition(@"DailyDscovr", td);
                }
                else
                {
                    Console.WriteLine("Removing task....");
                    ts.RootFolder.DeleteTask("DailyDscovr");
                }

                Console.WriteLine("Success!");
            }

            return 1;
        }
    }
}

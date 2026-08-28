using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Toolkit.Uwp.Notifications;
using System.IO;

namespace SleepyTime_2._0
{
    public static class Notifications
    {
        public static void ShowNotif(string title, string message, string iconPath = null, bool playsound = true)
        {
            try
            {
                var builder = new ToastContentBuilder()
                    .AddText(title)
                    .AddText(message);

                if(!string.IsNullOrEmpty(iconPath) && File.Exists(iconPath))
                {
                    builder.AddAppLogoOverride(new Uri($"file:///{iconPath}"));
                }

                if(!playsound)
                {
                    builder.AddAudio(null);
                }
                else
                {
                    builder.AddAudio(new Uri("ms-winsoundevent:Notification.Default"));
                }

                //NOT CURRENTLY WORKING
                //builder.Show();
            }
            catch(Exception e)
            {
                MessageBox.Show("Notification Failed: " + e.Message.ToString());
            }
        }
    }
}

using System;
using System.Windows.Forms;
using DiscordRPC;

namespace GameStatusSpoofer
{
    public class MainForm : Form
    {
        private TextBox txtGameName;
        private TextBox txtDetails;
        private TextBox txtState;
        private Button btnUpdate;
        private DiscordRpcClient client;

        public MainForm()
        {
            // Set up a native Windows Form window
            this.Text = "GameStatusSpoofer Control Panel";
            this.Width = 400;
            this.Height = 350;

            Label lblGame = new Label() { Text = "Game Name:", Left = 20, Top = 20, Width = 100 };
            txtGameName = new TextBox() { Left = 130, Top = 20, Width = 220 };

            Label lblDetails = new Label() { Text = "Details (Line 1):", Left = 20, Top = 60, Width = 100 };
            txtDetails = new TextBox() { Left = 130, Top = 60, Width = 220 };

            Label lblState = new Label() { Text = "State (Line 2):", Left = 20, Top = 100, Width = 100 };
            txtState = new TextBox() { Left = 130, Top = 100, Width = 220 };

            btnUpdate = new Button() { Text = "Update Presence", Left = 20, Top = 160, Width = 330, Height = 40 };
            btnUpdate.Click += BtnUpdate_Click;

            this.Controls.Add(lblGame); this.Controls.Add(txtGameName);
            this.Controls.Add(lblDetails); this.Controls.Add(txtDetails);
            this.Controls.Add(lblState); this.Controls.Add(txtState);
            this.Controls.Add(btnUpdate);
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            // Connect straight to the local Discord background process
            if (client != null) client.Dispose();

            client = new DiscordRpcClient("123456789012345678"); // Default App ID
            client.Initialize();

            client.SetPresence(new RichPresence()
            {
                Details = txtDetails.Text,
                State = txtState.Text,
                Assets = new Assets()
                {
                    LargeImageKey = "default_game",
                    LargeImageText = txtGameName.Text
                }
            });

            MessageBox.Show("Status updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new MainForm());
        }
    }
}

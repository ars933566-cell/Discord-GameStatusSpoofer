using System;
using System.IO;
using System.Windows.Forms;
using DiscordRPC;

namespace GameStatusSpoofer
{
    public class MainForm : Form
    {
        // Text Fields
        private TextBox txtAppId;
        private TextBox txtGameName;
        private TextBox txtDetails;
        private TextBox txtState;
        private TextBox txtImageUrl;

        // Radio Buttons for Time Options
        private RadioButton rdoAppStarted;
        private RadioButton rdoLocalTime;
        private RadioButton rdoCustom;

        // Custom Date and Time Selectors
        private DateTimePicker dtpCustomDate;
        private DateTimePicker dtpCustomTime;

        // Action Buttons
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
        private DiscordRpcClient client;

        public MainForm()
        {
            // Dark UI layout configuration
            this.Text = "GameStatusSpoofer Control Panel";
            this.Width = 460;
            this.Height = 680; // Expanded to fit calendars and clocks
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = System.Drawing.Color.FromArgb(47, 49, 54); // Discord Dark
            this.ForeColor = System.Drawing.Color.White;

            System.Drawing.Font mainFont = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);

            // 1. Application ID
            Label lblAppId = new Label() { Text = "Discord Application ID:", Left = 20, Top = 20, Width = 160, Font = mainFont };
            txtAppId = new TextBox() { Left = 190, Top = 18, Width = 230, Font = mainFont, BackColor = System.Drawing.Color.FromArgb(32, 34, 37), ForeColor = System.Drawing.Color.White };

            // 2. Game Name
            Label lblGame = new Label() { Text = "Game Name (Hover Text):", Left = 20, Top = 65, Width = 160, Font = mainFont };
            txtGameName = new TextBox() { Left = 190, Top = 63, Width = 230, Font = mainFont, BackColor = System.Drawing.Color.FromArgb(32, 34, 37), ForeColor = System.Drawing.Color.White };

            // 3. Details
            Label lblDetails = new Label() { Text = "Details (Line 1):", Left = 20, Top = 110, Width = 160, Font = mainFont };
            txtDetails = new TextBox() { Left = 190, Top = 108, Width = 230, Font = mainFont, BackColor = System.Drawing.Color.FromArgb(32, 34, 37), ForeColor = System.Drawing.Color.White };

            // 4. State
            Label lblState = new Label() { Text = "State (Line 2):", Left = 20, Top = 155, Width = 160, Font = mainFont };
            txtState = new TextBox() { Left = 190, Top = 153, Width = 230, Font = mainFont, BackColor = System.Drawing.Color.FromArgb(32, 34, 37), ForeColor = System.Drawing.Color.White };

            // 5. Image Link
            Label lblImage = new Label() { Text = "Large Image Link (URL):", Left = 20, Top = 200, Width = 160, Font = mainFont };
            txtImageUrl = new TextBox() { Left = 190, Top = 198, Width = 230, Font = mainFont, BackColor = System.Drawing.Color.FromArgb(32, 34, 37), ForeColor = System.Drawing.Color.White };

            // --- EXPLICIT TIMESTAMP PICKER GROUP ---
            GroupBox grpTimestamp = new GroupBox() { Text = "Timestamp Options", Left = 20, Top = 245, Width = 400, Height = 210, Font = mainFont, ForeColor = System.Drawing.Color.DarkGray };
            
            rdoAppStarted = new RadioButton() { Text = "Since app started (Standard Elapsed)", Left = 15, Top = 25, Width = 350, Checked = true, ForeColor = System.Drawing.Color.White };
            rdoLocalTime = new RadioButton() { Text = "Your current local time clock", Left = 15, Top = 55, Width = 350, ForeColor = System.Drawing.Color.White };
            rdoCustom = new RadioButton() { Text = "Custom Start Timestamp:", Left = 15, Top = 85, Width = 350, ForeColor = System.Drawing.Color.White };
            
            // Calendar Element (Year / Month / Day)
            Label lblCustomDate = new Label() { Text = "Select Date:", Left = 35, Top = 120, Width = 100, ForeColor = System.Drawing.Color.White, Font = new System.Drawing.Font("Segoe UI", 9F) };
            dtpCustomDate = new DateTimePicker() { Left = 140, Top = 116, Width = 230, Format = DateTimePickerFormat.Short, Enabled = false };
            
            // Clock Element (Hours / Minutes / Seconds)
            Label lblCustomTime = new Label() { Text = "Select Time:", Left = 35, Top = 160, Width = 100, ForeColor = System.Drawing.Color.White, Font = new System.Drawing.Font("Segoe UI", 9F) };
            dtpCustomTime = new DateTimePicker() { Left = 140, Top = 156, Width = 230, Format = DateTimePickerFormat.Time, ShowUpDown = true, Enabled = false };

            // Manage enabling logic dynamically based on selected option
            rdoCustom.CheckedChanged += (s, e) => {
                dtpCustomDate.Enabled = rdoCustom.Checked;
                dtpCustomTime.Enabled = rdoCustom.Checked;
            };

            grpTimestamp.Controls.Add(rdoAppStarted);
            grpTimestamp.Controls.Add(rdoLocalTime);
            grpTimestamp.Controls.Add(rdoCustom);
            grpTimestamp.Controls.Add(lblCustomDate); grpTimestamp.Controls.Add(dtpCustomDate);
            grpTimestamp.Controls.Add(lblCustomTime); grpTimestamp.Controls.Add(dtpCustomTime);

            // 6. SAVE Profile Button
            btnSave = new System.Windows.Forms.Button()
            {
                Text = "💾 Save Profile",
                Left = 20, Top = 480, Width = 190, Height = 35,
                Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold),
                BackColor = System.Drawing.Color.FromArgb(79, 84, 92), FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            // 7. LOAD Profile Button
            btnLoad = new System.Windows.Forms.Button()
            {
                Text = "📂 Load Profile",
                Left = 230, Top = 480, Width = 190, Height = 35,
                Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold),
                BackColor = System.Drawing.Color.FromArgb(79, 84, 92), FlatStyle = FlatStyle.Flat
            };
            btnLoad.FlatAppearance.BorderSize = 0;
            btnLoad.Click += BtnLoad_Click;

            // 8. Connect & Update Action Button
            btnUpdate = new System.Windows.Forms.Button() 
            { 
                Text = "Connect & Update Presence", 
                Left = 20, Top = 535, Width = 400, Height = 50, 
                Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold),
                BackColor = System.Drawing.Color.FromArgb(88, 101, 242), // Discord Blurple
                FlatStyle = FlatStyle.Flat
            };
            btnUpdate.FlatAppearance.BorderSize = 0;
            btnUpdate.Click += BtnUpdate_Click;

            // Render elements onto layout canvas
            this.Controls.Add(lblAppId); this.Controls.Add(txtAppId);
            this.Controls.Add(lblGame); this.Controls.Add(txtGameName);
            this.Controls.Add(lblDetails); this.Controls.Add(txtDetails);
            this.Controls.Add(lblState); this.Controls.Add(txtState);
            this.Controls.Add(lblImage); this.Controls.Add(txtImageUrl);
            this.Controls.Add(grpTimestamp);
            this.Controls.Add(btnSave); this.Controls.Add(btnLoad);
            this.Controls.Add(btnUpdate);
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAppId.Text))
            {
                MessageBox.Show("Please enter a valid Discord Application ID first!", "Missing ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (client != null) client.Dispose();

                client = new DiscordRpcClient(txtAppId.Text.Trim());
                client.Initialize();

                System.Threading.Thread.Sleep(600);

                if (client.IsInitialized)
                {
                    var presence = new RichPresence()
                    {
                        Details = txtDetails.Text,
                        State = txtState.Text,
                        Assets = new Assets()
                        {
                            LargeImageKey = string.IsNullOrWhiteSpace(txtImageUrl.Text) ? "default" : txtImageUrl.Text.Trim(),
                            LargeImageText = txtGameName.Text
                        }
                    };

                    // --- CALENDAR TIME CONVERSION LOGIC ---
                    if (rdoAppStarted.Checked)
                    {
                        presence.Timestamps = Timestamps.Now;
                    }
                    else if (rdoLocalTime.Checked)
                    {
                        presence.Timestamps = new Timestamps(DateTime.UtcNow);
                    }
                    else if (rdoCustom.Checked)
                    {
                        // Extract specific data targets chosen by UI Pickers
                        DateTime selectedDate = dtpCustomDate.Value;
                        DateTime selectedTime = dtpCustomTime.Value;

                        // Combine into a singular absolute point in history
                        DateTime combinedDateTime = new DateTime(
                            selectedDate.Year, selectedDate.Month, selectedDate.Day,
                            selectedTime.Hour, selectedTime.Minute, selectedTime.Second
                        );

                        // Feed directly to Discord API as universal timestamp coordinates
                        presence.Timestamps = new Timestamps()
                        {
                            Start = combinedDateTime.ToUniversalTime()
                        };
                    }

                    client.SetPresence(presence);
                    MessageBox.Show("Success! Your custom target timestamp strategy is active.", "Status Spoofer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Connection failed! Ensure Desktop Discord app is running in the background.", "Discord Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error running execution string: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Profile (*.gsp)|*.gsp" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string[] lines = { 
                        txtAppId.Text, txtGameName.Text, txtDetails.Text, txtState.Text, txtImageUrl.Text, 
                        rdoAppStarted.Checked.ToString(), rdoLocalTime.Checked.ToString(), rdoCustom.Checked.ToString(),
                        dtpCustomDate.Value.ToString("yyyy-MM-dd"), dtpCustomTime.Value.ToString("HH:mm:ss")
                    };
                    File.WriteAllLines(sfd.FileName, lines);
                }
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "Profile (*.gsp)|*.gsp" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string[] lines = File.ReadAllLines(ofd.FileName);
                    if (lines.Length >= 5) { txtAppId.Text = lines[0]; txtGameName.Text = lines[1]; txtDetails.Text = lines[2]; txtState.Text = lines[3]; txtImageUrl.Text = lines[4]; }
                    if (lines.Length >= 8) { rdoAppStarted.Checked = bool.Parse(lines[5]); rdoLocalTime.Checked = bool.Parse(lines[6]); rdoCustom.Checked = bool.Parse(lines[7]); }
                    if (lines.Length >= 10)
                    {
                        dtpCustomDate.Value = DateTime.Parse(lines[8]);
                        dtpCustomTime.Value = DateTime.Parse(lines[9]);
                    }
                }
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}

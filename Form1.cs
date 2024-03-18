using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;


//metallic image credit "https://www.freepik.com/free-photo/metallic-background-with-grunge-scratched-effect_13839423.htm#query=rough%20metal%20texture&position=1&from_view=keyword&track=ais&uuid=857e71e6-eb1c-4174-ade6-69d2ddc255c0" Image by kjpargeter</a> on Freepik
//red metallic image credit "https://www.freepik.com/free-photo/colourful-wall-seamless-background-texture_5458608.htm#fromView=search&page=1&position=1&uuid=f17c5890-96dc-4b52-b939-af6f26a8c281" Image by freepik
//yellow metallic image credit "https://www.freepik.com/free-photo/yellow-painted-wall-background_13299816.htm#fromView=search&page=1&position=1&uuid=6979c5da-244c-4f8d-b1b2-91a27aaa0bb0"Image by rawpixel.com on Freepik
//green mettalic image credit"https://www.freepik.com/free-photo/green-granite-wall-background_5281067.htm#fromView=search&page=1&position=35&uuid=6753b319-d0e1-4441-9472-eb35303b3740" Image by freepik<
//metal1 image credit "https://www.freepik.com/free-photo/flat-lay-metal-surface_11684377.htm#query=rough%20metal%20texture&position=0&from_view=keyword&track=ais&uuid=c1e89dc6-9fb2-46dc-a5b6-f38ce05a3d9b"
//metal2 image credit "https://www.freepik.com/free-photo/scratched-steel-textured-background-design_17114169.htm#query=rough%20metal%20texture&position=29&from_view=keyword&track=ais&uuid=6b4afb88-0391-4346-aa7f-e0aec521a212"


namespace ATM_simulator
{
    /**
     * Main ATM class which is a form
     */
    public partial class ATMForm : Form
    {
        // set up GUI variables
        private System.Windows.Forms.Button btnWithdraw, btnCheckBalance, btnLogout;
        private Label lblBalance;
        private System.Windows.Forms.Button btnReturntoMenu, btnLockedLogin;
        private Label lblWithdrawInstructions;
        private ATMState currentState = ATMState.LoggedOut;
        private System.Windows.Forms.Button btnWithdraw10;
        private System.Windows.Forms.Button btnWithdraw50;
        private System.Windows.Forms.Button btnWithdraw500;
        private System.Windows.Forms.Button btnWithdrawCustom;
        private System.Windows.Forms.Button btnEnter, btnCancel, btnClear, btnBlank;
        private System.Windows.Forms.Label lblKeypad, lblLoginText, lblOptionsText, lblWelcome, lblProcessWithdraw;
        private System.Windows.Forms.Panel pnlScreenBlock, pnlKeypad;
        private Label lblAccEnter, lblPinEnter;
        Label lblEnterCustomWithdraw, lblWithdrawOutcome;
        private string pinText;
        private Label lblMaxAttempts;
        private int? enteredAcc = null;
        private PictureBox lockPictureBox;
        private System.Windows.Forms.Timer sessionTimer;
        private Label lblTimeout;
        private System.Windows.Forms.Button btnTimeoutLogIn;


        // create 2D arrays of buttons
        private System.Windows.Forms.Button[,] btn;
        private System.Windows.Forms.Button[] btnSelectLeft, btnSelectRight;

        // local reference to array of accounts
        private Account[] ac;
        private bool dataRace;

        //this is a referance to the account that is being used
        private Account activeAccount = null;

        // setting starting attemps
        private int accountNumberAttempts = 0;
        //private int pinAttempts = 0;

        // setting maximum attempts
        private const int MaxAccountNumberAttempts = 5;
        private const int MaxPinAttempts = 3;


        // the atm constructor takes an array of account objects as a reference
        // also takes a boolean value which tells it whether it is in a data race or not
        public ATMForm(Account[] ac, bool dataRace)
        {
            InitializeComponent();

            InitializeControls();
            this.ac = ac;
            this.dataRace = dataRace;

            this.BackColor = Color.White;
            this.Size = new Size(900, 600);
            InitializeLogin();

            InitializeTimer();


        }

        /**
         * Find an account by its account number
         * accountNumber - the account number to find
         */
        public Account findAccount(int accountNumber)
        {
            foreach (Account acc in ac)
            {
                if (acc.GetAccountNum() == accountNumber)
                {
                    return acc;
                }
            }
            return null;
        }

        /**
         * Variable to store which state the ATM is currently
         */
        enum ATMState
        {
            LoggedIn,
            EnteredAccount,
            LoggedOut,
            DisplayingBalance,
            WithdrawingMoney,
            ProcessingWithdrawal,
            TimedOut,

        }

        /**
         * Initialise all of the controls for using the system once the user has logged in
         */
        private void InitializeControls()
        {
            lblOptionsText = new Label { Text = "Please select an option listed:", ForeColor = Color.White, BackColor = Color.DodgerBlue, AutoEllipsis = false, AutoSize = true, BorderStyle = BorderStyle.None, Font = new Font("Arial", 20, FontStyle.Regular), MaximumSize = new Size(400, 60), Location = new Point(120, 60), Visible = false };
            Controls.Add(lblOptionsText);
            lblOptionsText.BringToFront();

            btnWithdraw = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 18, FontStyle.Regular), Text = "Withdraw", Visible = false, Location = new Point(80, 110), Size = new Size(220, 50) };
            btnCheckBalance = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 18, FontStyle.Regular), Text = "Check Balance", Visible = false, Location = new Point(80, 170), Size = new Size(220, 50) };
            btnLogout = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 18, FontStyle.Regular), Text = "Logout", Visible = false, Location = new Point(80, 230), Size = new Size(220, 50) };
            lblBalance = new Label { Location = new Point(120, 100), Size = new Size(400, 60), Visible = false, Font = new Font("Arial", 20, FontStyle.Regular), ForeColor = Color.White, BackColor = Color.DodgerBlue };
            lblWithdrawInstructions = new Label { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.DodgerBlue, Text = "Choose your withdrawal amount below: ", BorderStyle = BorderStyle.None, Location = new Point(70, 40), Size = new Size(400, 60), Visible = false, Font = new Font("Arial", 18, FontStyle.Regular) };

            btnWithdraw10 = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 24, FontStyle.Regular), Text = "£10", Location = new Point(80, 110), Size = new Size(220, 50), Visible = false };
            btnWithdraw50 = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 24, FontStyle.Regular), Text = "£50", Location = new Point(80, 170), Size = new Size(220, 50), Visible = false };
            btnWithdraw500 = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 24, FontStyle.Regular), Text = "£500", Location = new Point(80, 230), Size = new Size(220, 50), Visible = false };
            btnWithdrawCustom = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 18, FontStyle.Regular), Text = "Custom Amount", Location = new Point(320, 110), Size = new Size(220, 50), Visible = false };
            btnReturntoMenu = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 12, FontStyle.Regular), Text = "Return to Account Menu", Size = new Size(220, 50), Location = new Point(320, 230), Visible = false };

            lblProcessWithdraw = new Label { Text = "Please wait, contacting your bank... ↻", ForeColor = Color.White, BackColor = Color.DodgerBlue, AutoEllipsis = false, AutoSize = true, BorderStyle = BorderStyle.None, Font = new Font("Arial", 26, FontStyle.Regular), MaximumSize = new Size(500, 200), Location = new Point(100, 100), Visible = false };
            lblEnterCustomWithdraw = new Label { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.DodgerBlue, Text = "Use the keypad to enter a custom withdrawal amount: ", BorderStyle = BorderStyle.None, Location = new Point(80, 100), Size = new Size(500, 200), Visible = false, Font = new Font("Arial", 24, FontStyle.Regular) };
            lblWithdrawOutcome = new Label { ForeColor = Color.White, BackColor = Color.DodgerBlue, AutoEllipsis = false, AutoSize = true, BorderStyle = BorderStyle.None, Font = new Font("Arial", 26, FontStyle.Regular), MaximumSize = new Size(500, 200), Location = new Point(100, 100), Visible = false };
     


            // add functionality to buttons
            btnWithdraw.Click += new EventHandler(this.btnWithdraw_Click);
            btnCheckBalance.Click += new EventHandler(this.btnCheckBalance_Click);
            btnLogout.Click += new EventHandler(this.btnLogout_Click);
            btnReturntoMenu.Click += new EventHandler(this.btnReturntoMenu_Click);

            btnWithdraw10.Click += new EventHandler(this.btnWithdraw10_Click);
            btnWithdraw50.Click += new EventHandler(this.btnWithdraw50_Click);

            btnWithdraw500.Click += new EventHandler(this.btnWithdraw500_Click);
            btnWithdrawCustom.Click += new EventHandler(this.btnWithdrawCustom_Click);

            Controls.Add(lblBalance);
            Controls.Add(btnWithdraw);
            Controls.Add(btnCheckBalance);
            Controls.Add(btnLogout);
            Controls.Add(btnReturntoMenu);
            Controls.Add(lblWithdrawInstructions);
            Controls.Add(btnWithdraw10);
            Controls.Add(btnWithdraw50);
            Controls.Add(btnWithdraw500);
            Controls.Add(btnWithdrawCustom);
            Controls.Add(lblProcessWithdraw);
            Controls.Add(lblEnterCustomWithdraw);
            Controls.Add(lblWithdrawOutcome);


            lblTimeout = new Label { Text = "Your session has timed out. ⏱︎ ", ForeColor = Color.White, BackColor = Color.DodgerBlue, Font = new Font("Arial", 20, FontStyle.Regular), BorderStyle = BorderStyle.None, AutoSize = true, AutoEllipsis = false, MaximumSize = new Size(400, 900), Location = new Point(120, 60), Visible = false };
            Controls.Add(lblTimeout);
            lblTimeout.BringToFront();

            btnTimeoutLogIn = new System.Windows.Forms.Button { Text = "Log Back In?", ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 12, FontStyle.Regular), Size = new Size(220, 50), Location = new Point(320, 230), Visible = false };
            btnTimeoutLogIn.Click += new EventHandler(this.BtnTimeoutLogin_Click);
            Controls.Add(btnTimeoutLogIn);

        }

        /**
         * Initialise all of the controls to allow the user to log into their account
         */
        private void InitializeLogin()
        {

            // create a colored block for the screen       
            pnlScreenBlock = new Panel { BackColor = Color.DodgerBlue, Height = 300, Width = 525, Location = new Point((this.Width) / 16, (this.Height - Height) / 2), BorderStyle = BorderStyle.Fixed3D };
            Controls.Add(pnlScreenBlock);
            pnlScreenBlock.SendToBack();

            // panel for the keypad height is 360 Dock = DockStyle.Bottom,
            pnlKeypad = new Panel { BackColor = Color.Silver, Width = this.Width, Height = 700, BackgroundImage = Properties.Resources.metallic, BackgroundImageLayout = ImageLayout.Stretch };
            Controls.Add(pnlKeypad);


            // create keypad
            btn = new System.Windows.Forms.Button[3, 4];
            // set up buttons
            int num = 1;
            for (int y = 0; y < btn.GetLength(1); y++)
            {
                for (int x = 0; x < btn.GetLength(0); x++)
                {
                    btn[x, y] = new System.Windows.Forms.Button { Location = new Point(90 + (80 * x), 310 + (80 * y)), Height = 75, Width = 75, BackColor = Color.LightSlateGray, FlatStyle = FlatStyle.Popup, Font = new Font("Arial", 20, FontStyle.Regular), BackgroundImage = Properties.Resources.metalButt };
                    btn[x, y].Click += new EventHandler(this.btnEvent_Click);
                    if (y < 3)
                    {
                        btn[x, y].Text = Convert.ToString(num);
                    }
                    else
                    {
                        if (x == 1)
                        {
                            btn[x, y].Text = "0";
                        }
                        else
                        {
                            btn[x, y].Text = "";
                        }
                    }
                    Controls.Add(btn[x, y]);
                    btn[x, y].BringToFront();
                    num++;
                }
            }

            // create buttons to select options on the screen
            btnSelectLeft = new System.Windows.Forms.Button[3];
            // set up buttons
            num = 1;
            for (int x = 0; x < btnSelectLeft.GetLength(0); x++)
            {
                btnSelectLeft[x] = new System.Windows.Forms.Button { Location = new Point(5, 120 + (60 * x)), Height = 40, Width = 40, BackColor = Color.LightSlateGray, FlatStyle = FlatStyle.Popup, BackgroundImage = Properties.Resources.metal2, BackgroundImageLayout = ImageLayout.Stretch };
                Controls.Add(btnSelectLeft[x]);
                btnSelectLeft[x].BringToFront();
                num++;
            }

            btnSelectRight = new System.Windows.Forms.Button[3];
            // set up buttons
            num = 1;
            for (int x = 0; x < btnSelectRight.GetLength(0); x++)
            {
                btnSelectRight[x] = new System.Windows.Forms.Button { Location = new Point(590, 120 + (60 * x)), Height = 40, Width = 40, BackColor = Color.LightSlateGray, FlatStyle = FlatStyle.Popup, BackgroundImage = Properties.Resources.metal2, BackgroundImageLayout = ImageLayout.Stretch };
                Controls.Add(btnSelectRight[x]);
                btnSelectRight[x].BringToFront();
                num++;
            }


            // lock picture
            lockPictureBox = new PictureBox { Size = new Size(width: 90, height: 90), BackColor = this.BackColor, SizeMode = PictureBoxSizeMode.Zoom, Location = new Point(255, 10), Visible = false, Image = Properties.Resources.padlock }; Controls.Add(lockPictureBox);
            lockPictureBox.BringToFront();

            //add text
            lblLoginText = new Label { Text = "Login to your account to start", ForeColor = Color.White, BackColor = Color.DodgerBlue, Font = new Font("Arial", 20, FontStyle.Regular), BorderStyle = BorderStyle.None, AutoSize = true, AutoEllipsis = false, MaximumSize = new Size(400, 900), Location = new Point(120, 60) };
            Controls.Add(lblLoginText);
            lblLoginText.BringToFront();

            //add text
            lblWelcome = new Label { ForeColor = Color.White, BackColor = Color.DodgerBlue, Font = new Font("Arial", 20, FontStyle.Regular), BorderStyle = BorderStyle.None, AutoSize = true, AutoEllipsis = false, MaximumSize = new Size(400, 900), Location = new Point(120, 60) };
            Controls.Add(lblLoginText);
            lblLoginText.BringToFront();

            //display enter account message
            lblAccEnter = new Label { Text = "Use the keypad to enter your 6-digit account number 🔑: ", Font = new Font("Arial", 20, FontStyle.Regular), Location = new Point(120, 120), Size = new Size(400, 100), BackColor = Color.DodgerBlue, ForeColor = Color.White };
            Controls.Add(lblAccEnter);
            lblAccEnter.BringToFront();

            //text box for pin and account number
            lblKeypad = new System.Windows.Forms.Label { Location = new Point(120, 200), Size = new Size(350, 40), Font = new Font("Arial", 26, FontStyle.Regular), BorderStyle = BorderStyle.None, BackColor = Color.Silver, ForeColor = Color.Black };
            Controls.Add(lblKeypad);
            lblKeypad.BringToFront();

            // cancel button 
            btnCancel = new System.Windows.Forms.Button { Text = "Cancel", ForeColor = Color.Black, FlatStyle = FlatStyle.Popup, Font = new Font("Arial", 20, FontStyle.Regular), Location = new Point(350, 310), Size = new Size(150, 75), BackColor = Color.Red, BackgroundImage = Properties.Resources.metalRed, BackgroundImageLayout = ImageLayout.Stretch };
            btnCancel.Click += new EventHandler(this.btnCancel_Click);
            Controls.Add(this.btnCancel);
            btnCancel.BringToFront();

            // clear  button
            btnClear = new System.Windows.Forms.Button { Text = "Clear", ForeColor = Color.Black, FlatStyle = FlatStyle.Popup, Font = new Font("Arial", 20, FontStyle.Regular), Location = new Point(350, 390), Size = new Size(150, 75), BackColor = Color.Yellow, BackgroundImage = Properties.Resources.metalYellow, BackgroundImageLayout = ImageLayout.Stretch };
            btnClear.Click += new EventHandler(this.btnClear_Click);
            Controls.Add(this.btnClear);
            btnClear.BringToFront();

            // enter button
            btnEnter = new System.Windows.Forms.Button { Text = "Enter", ForeColor = Color.Black, FlatStyle = FlatStyle.Popup, Font = new Font("Arial", 20, FontStyle.Regular), Location = new Point(350, 470), Size = new Size(150, 75), BackColor = Color.Lime, BackgroundImage = Properties.Resources.metalGreen, BackgroundImageLayout = ImageLayout.Stretch };
            btnEnter.Click += new EventHandler(this.btnEnter_Click);
            Controls.Add(this.btnEnter);
            btnEnter.BringToFront();

            // blank button
            btnBlank = new System.Windows.Forms.Button { Text = "", ForeColor = Color.Black, FlatStyle = FlatStyle.Popup, Font = new Font("Arial", 20, FontStyle.Regular), Location = new Point(350, 550), Size = new Size(150, 75), BackColor = Color.LightSlateGray, BackgroundImage = Properties.Resources.metal1, BackgroundImageLayout = ImageLayout.Stretch };
            Controls.Add(this.btnBlank);
            btnBlank.BringToFront();

            //display a message to enter the user's pin
            lblPinEnter = new Label { Text = "Please enter your 4-digit pin 🔓:", Font = new Font("Arial", 20, FontStyle.Regular), Location = new Point(120, 120), Size = new Size(400, 80), BackColor = Color.DodgerBlue, ForeColor = Color.White, Visible = false };
            Controls.Add(lblPinEnter);
            lblPinEnter.BringToFront();

            btnLockedLogin = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 12, FontStyle.Regular), Text = "Login to a different account", Size = new Size(220, 50), Location = new Point(320, 230), Visible = false };
            btnLockedLogin.Click += new EventHandler(this.btnLockedLogin_Click);
            Controls.Add(btnLockedLogin);
            btnLockedLogin.BringToFront();


            lblMaxAttempts = new Label { Text = "Please enter your 4-digit pin 🔓:", Font = new Font("Arial", 20, FontStyle.Regular), Location = new Point(120, 120), Size = new Size(400, 80), BackColor = Color.DodgerBlue, ForeColor = Color.White, Visible = false };
            Controls.Add(lblMaxAttempts);
            lblMaxAttempts.BringToFront();

        }


        /**
         * Timer to timeout atm session and log user out when user is inactive
         */
        private void InitializeTimer()
        {
            sessionTimer = new System.Windows.Forms.Timer { Interval = 60000 };
            sessionTimer.Tick += SessionTimer_Tick;
        }

        /**
         * Stop the timer and log the user out of their session if they have been inactive 
         */
        private void SessionTimer_Tick(object sender, EventArgs e)
        {
            sessionTimer.Stop();

            // logging the user out
            currentState = ATMState.TimedOut;
            updateUI(currentState);


        }

        /**
         * Button to log back in again after the user has been timed out
         */
        private void BtnTimeoutLogin_Click(object sender, EventArgs e)
        {
            lblTimeout.Visible = false;
            btnTimeoutLogIn.Visible = false;

            currentState = ATMState.LoggedOut;
            updateUI(currentState);
        }

        /**
         * Button to log back in again to a different account after being locked out of an account
         */
        private void  btnLockedLogin_Click(object sender, EventArgs e)
        {
            // enable all buttons
            foreach (var button in this.Controls.OfType<System.Windows.Forms.Button>())
            {
                button.Enabled = true;
            }
            btnLockedLogin.Visible = false;

            currentState = ATMState.LoggedOut;
            updateUI(currentState);
        }

        /**
         * Reset the session timer once the user becomes active again
         */
        private void ResetSessionTimer()
        {
            if (currentState == ATMState.LoggedIn)
            {
                sessionTimer.Stop();
                sessionTimer.Start();
            }
        }

        /**
         * Event handler for keypad buttons
         * Make text appear oninput label
         */
        void btnEvent_Click(object sender, EventArgs e)
        {
            // display *s if the user is entering their password
            if (currentState == ATMState.EnteredAccount)
            {
                pinText += ((System.Windows.Forms.Button)sender).Text;
                lblKeypad.Text += "*";
                Console.WriteLine(pinText);
            }
            else
            {
                lblKeypad.Text += ((System.Windows.Forms.Button)sender).Text;
            }

            // user interaction, reset the timer
            ResetSessionTimer();

        }

        /**
         * Clear text from the enter info label
         */
        void btnClear_Click(object sender, EventArgs e)
        {
            lblKeypad.Text = "";
            pinText = "";

            ResetSessionTimer();
        }

        /**
         * Event handler for cancel button
         * Return to start of login page if not logged in and return to logged in page if already logged in
         */
        void btnCancel_Click(object sender, EventArgs e)
        {
            if (currentState == ATMState.EnteredAccount || currentState == ATMState.LoggedOut)
            {
                currentState = ATMState.LoggedOut;
                updateUI(currentState);
            }
            else
            {
                currentState = ATMState.LoggedIn;
                updateUI(currentState);
            }
            
            ResetSessionTimer();

        }

        /**
         * Event handler for button to withdraw a custom amount
         * Ask the user to input their custome amount
         */
        private void btnWithdrawCustom_Click(object sender, EventArgs e)
        {
            resetUI();
            lblEnterCustomWithdraw.Visible = true;
            lblKeypad.Visible = true;

            ResetSessionTimer();
        }

        /**
         * Event handler for button to withdraw £50
         */
        private void btnWithdraw50_Click(object sender, EventArgs e)
        {
            doWithdrawal(50);

            ResetSessionTimer();
        }

        /**
         * Event handler for button to withdraw £500
         */
        private void btnWithdraw500_Click(object sender, EventArgs e)
        {
            doWithdrawal(500);

            ResetSessionTimer();
        }

        /**
         * Event handler for button to withdraw £10
         */

        private void btnWithdraw10_Click(object sender, EventArgs e)
        {
            doWithdrawal(10);

            ResetSessionTimer();
        }

        /**
         * Event handler for button to return to the main logged in menu
         */
        private void btnReturntoMenu_Click(object sender, EventArgs e)
        {
            currentState = ATMState.LoggedIn;
            updateUI(currentState);

            ResetSessionTimer();
        }

        /**
         * Reset the user interface by removing everything from it
         */
        private void resetUI()
        {
            //Code generated objects 
            btnCheckBalance.Visible = false;
            btnWithdraw.Visible = false;
            btnLogout.Visible = false;
            lblBalance.Visible = false;
            btnReturntoMenu.Visible = false;
            lblWithdrawInstructions.Visible = false;
            btnWithdraw10.Visible = false;
            btnWithdraw50.Visible = false;
            btnWithdraw500.Visible = false;
            btnWithdrawCustom.Visible = false;
            lblPinEnter.Visible = false;
            lblAccEnter.Visible = false;
            lblKeypad.Visible = false;
            lblLoginText.Visible = false;
            lblOptionsText.Visible = false;
            lblWelcome.Visible = false;
            lblProcessWithdraw.Visible = false;
            lblEnterCustomWithdraw.Visible = false;
            lblWithdrawOutcome.Visible = false;
            lblKeypad.Text = "";
            lockPictureBox.Visible = false;
            lblTimeout.Visible = false;
            btnTimeoutLogIn.Visible = false;
            btnLockedLogin.Visible = false;
            lblMaxAttempts.Visible = false;


            // remove event handlers form side buttons
            btnSelectLeft[0].Click -= btnWithdraw_Click;
            btnSelectLeft[1].Click -= btnCheckBalance_Click;
            btnSelectLeft[2].Click -= btnLogout_Click;
            btnSelectRight[0].Click -= btnWithdrawCustom_Click;
            btnSelectRight[2].Click -= btnReturntoMenu_Click;
            btnSelectLeft[0].Click -= btnWithdraw10_Click;
            btnSelectLeft[1].Click -= btnWithdraw50_Click;
            btnSelectLeft[2].Click -= btnWithdraw500_Click;
            btnSelectRight[2].Click -= BtnTimeoutLogin_Click;
            btnSelectRight[2].Click -= btnLockedLogin_Click;
        }



        /**
         * Event handler to log a user out of their account and return to the initial login screen
         */
        private void btnLogout_Click(object sender, EventArgs e)
        {

            currentState = ATMState.LoggedOut;
            updateUI(currentState);


            sessionTimer.Stop();
        }
        
        /**
         * Event handler for check balance button
         * Allow the user to check and view their account balance
         */
        private void btnCheckBalance_Click(object sender, EventArgs e)
        {
            currentState = ATMState.DisplayingBalance;
            updateUI(currentState);

            if (this.activeAccount != null)
            {
                this.lblBalance.Text = "Your current balance is : " + activeAccount.getBalance();
            }
            ResetSessionTimer();
        }

        /**
         * Gives the user the option to choose an amount of money to withdraw from their account
         */

        private void btnWithdraw_Click(object sender, EventArgs e)
        {
            currentState = ATMState.WithdrawingMoney; // update UI to give option to withdraw money
            updateUI(currentState);

            ResetSessionTimer();
        }

        /**
         * Method to do the withdrawal from the active account, keeping a receipt of the trasaction and checking there are sufficient funds
         */
        private void doWithdrawal(int amount)
        {
            currentState = ATMState.ProcessingWithdrawal;
            updateUI(currentState);
            lblProcessWithdraw.Visible = true;
            Application.DoEvents(); // redraw GUI before doing withdrawal

            Thread.Sleep(2000);

            if (activeAccount != null && activeAccount.decrementBalance(amount, dataRace))
            {
                lblWithdrawOutcome.Text = $"Withdrawal successful \nNew balance : £{activeAccount.getBalance()} ";
                lblWithdrawOutcome.Visible = true;
                lblProcessWithdraw.Visible = false;
                btnSelectRight[2].Click += btnReturntoMenu_Click;

                // making reciept detaisl 
                string receiptDetails = receiptLog(activeAccount, "Withdrawal", amount);
                receiptSave(receiptDetails);
            }
            else
            {
                lblWithdrawOutcome.Text = "Insufficient funds";
                lblWithdrawOutcome.Visible = true;
                lblProcessWithdraw.Visible = false;
            }
            btnReturntoMenu.Visible = true;
        }


        /**
         * Log to keep activity of transcations, in this case a withdrawal
         */
        private string receiptLog(Account account, string transcationType, int amount)
        {

            StringBuilder receipt = new StringBuilder();

            receipt.AppendLine("ATM Transaction Receipt");
            receipt.AppendLine($"Account Number: {account.GetAccountNum()}");
            receipt.AppendLine($"Date/Time: {DateTime.Now}");
            receipt.AppendLine($"Transaction Type: {transcationType}");
            receipt.AppendLine($"Amount: £{amount:C}");
            receipt.AppendLine($"Remaning Balance:{account.getBalance():C}");
            receipt.AppendLine("");

            return receipt.ToString();
        }


        /**
         * MEthod to save the transaction receipt to a text file
         */
        private void receiptSave(string receiptDetails)
        {
            string filepath = "ATM_Receipts.txt";

            File.AppendAllText(filepath, receiptDetails + Environment.NewLine + "----------------------------" + Environment.NewLine);



        }

        /** 
         * Event handler for enter button
         * Checks what state the ATM is in to know what the user is using the enter button for
         * Handles account number entry, pin number entry, and custom withdrawal ammount
         */
        private void btnEnter_Click(object sender, EventArgs e)
        {

            // first check what enter is being used for - if entering acount number, pin, or custom amount
            if (currentState == ATMState.LoggedOut) // user is entering account number
            {
                /*
                if (++accountNumberAttempts > MaxAccountNumberAttempts)
                {
                    //MaximumAccNumAttempts();
                    return;
                }
                */

                if (string.IsNullOrWhiteSpace(lblKeypad.Text))
                {
                    Shake(lblKeypad, 500, 5);
                    lblAccEnter.Text = ("Enter your account number before pressing enter 🔑:");
                    return;
                }

                if (!int.TryParse(lblKeypad.Text, out int accountNumber) || accountNumber <= 0)
                {
                    Shake(lblKeypad, 500, 5);
                    lblAccEnter.Text = ("Please enter a valid 6-digit account number 🔑:");
                    lblKeypad.Text = "";
                    return;
                }

                activeAccount = findAccount(int.Parse(lblKeypad.Text));
                if (activeAccount != null && activeAccount.getLocked() == false)
                {
                    // if we reach here, it is a valid account and is not locked, so display request for pin
                    enteredAcc = int.Parse(lblKeypad.Text);
                    lblKeypad.Text = "";
                    currentState = ATMState.EnteredAccount;
                    updateUI(currentState);
                    return;
                }
                else if (activeAccount.getLocked() == true)
                {
                    // the acccount has previously been locked and cannot be accessed 
                    lblMaxAttempts.Text = "This account has been locked and cannot be accessed 🔓:";
                    lblMaxAttempts.Visible = true;
                    lblAccEnter.Visible = false;
                    lblKeypad.Text = "";
                    // disable all buttons
                    foreach (var button in this.Controls.OfType<System.Windows.Forms.Button>())
                    {
                        button.Enabled = false;
                    }
                    btnLockedLogin.Enabled = true;
                    btnLockedLogin.Visible = true;
                    btnSelectRight[2].Click += btnLockedLogin_Click;
                    btnSelectRight[2].Enabled = true;
                    lblPinEnter.Visible = false;
                    lblKeypad.Visible = false;  
                    lblLoginText.Visible = false;
                    lockPictureBox.Visible = true;
                    lockPictureBox.Invalidate();
                }
                else
                {
                    // account number entered is invalid
                    Shake(lblKeypad, 500, 5);
                    lblAccEnter.Text = "Please enter a valid 6-digit account number 🔑:";
                    lblKeypad.Text = "";
                    return;
                }
            }
            else if (currentState == ATMState.WithdrawingMoney) // user is entering a custom amount
            {

                if (string.IsNullOrWhiteSpace(lblKeypad.Text))
                {
                    lblEnterCustomWithdraw.Text = "Enter your custom amount before pressing enter: ";
                    return;
                }
                int customAmount = int.Parse(lblKeypad.Text);
                doWithdrawal(customAmount);
            }
            else // user is entering pin
            {
                // update the number of pin attempts for the current active account
                int attempts = activeAccount.getPinAttempts();
                attempts++;
                activeAccount.setPinAttempts(attempts);
                // check if they have reached the maximum pin attempts
                if (activeAccount.getPinAttempts() >= MaxPinAttempts)
                {
                    lblKeypad.Text = "";
                    pinText = "";
                    MaximumPinAttempts();
                    activeAccount.setLocked(true);
                    return;
                }

                if (string.IsNullOrWhiteSpace(lblKeypad.Text))
                {
                    Shake(lblKeypad, 500, 5);
                    lblPinEnter.Text = "Enter your pin number before pressing enter 🔓:";
                    pinText = "";
                    return;
                }
                if (!int.TryParse(pinText, out int pin) || pin <= 0)
                {
                    Shake(lblKeypad, 500, 5);
                    lblPinEnter.Text = "Please enter a valid 4-digit PIN 🔓:";
                    lblKeypad.Text = "";
                    pinText = "";
                    return;
                }
                // othwerwise, check that the pin matches the account
                if (activeAccount.checkPin(int.Parse(pinText)))
                {
                    sessionTimer.Start();

                    activeAccount.setPinAttempts(0); // reset pin attempts once user successfully logs in
                    lblKeypad.Text = "";
                    pinText = "";
                    currentState = ATMState.LoggedIn;
                    lblWelcome.Text = "Welcome, " + activeAccount.GetAccountNum() + "!";
                    updateUI(currentState);
                }
                else
                {
                    Shake(lblKeypad, 500, 5);
                    lblPinEnter.Text = "Please enter a valid 4-digit PIN 🔓:";
                    lblKeypad.Text = "";
                    pinText = "";
                }
            }
            ResetSessionTimer();
        }

        // might not be needed - don't think it is
        /*
        private void MaximumAccNumAttempts()
        {
            resetUI();

            foreach (var button in this.Controls.OfType<System.Windows.Forms.Button>())
            {
                button.Enabled = false; 
            }

            lblKeypad.Visible = true;
            lblKeypad.Text = "Out of Attempts";
           
        }
        */

        /**
         * Display account locked page if the use has entered maximum pin attempts
         */
        private void MaximumPinAttempts()
        {
            // disable all buttons
            foreach (var button in this.Controls.OfType<System.Windows.Forms.Button>())
            {
                button.Enabled = false;
            }
            btnLockedLogin.Enabled = true;
            lblPinEnter.Visible = false;
            lblMaxAttempts.Visible = true;
            lblMaxAttempts.Text = $"Account number {enteredAcc} has now been locked 🔒";
            lblKeypad.Visible = false;
            btnLockedLogin.Visible = true;
            btnSelectRight[2].Click += btnLockedLogin_Click;
            btnSelectRight[2].Enabled = true;


            lblLoginText.Visible = false;
            lockPictureBox.Visible = true;
            lockPictureBox.Invalidate();

        }

        /**
         * Method to shake a given control for a certain period of time
         * control - control to shake
         * time - time to shake the control for
         * amount - amount to shake the control
         */
        private void Shake(Control control, int time, int amount)
        {
            var normalLocation = control.Location;
            Random rnd = new Random();


            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            while (stopWatch.ElapsedMilliseconds < time)
            {
                // random offsets to cause a shake effect
                int x = normalLocation.X + rnd.Next(-amount, amount);
                int y = normalLocation.Y + rnd.Next(-amount, amount);
                control.Location = new Point(x, y);
                Thread.Sleep(10);
            }
            stopWatch.Stop();

            control.Location = normalLocation;

        }


        /**
         * Update the UI based on the state it is currently in to make it easier to control
         */
        private void updateUI(ATMState state)
        {
            // remove everything from the UI first
            resetUI();

            // then add back the items that will be needed for the relevant state
            switch (state)
            {
                case ATMState.LoggedIn:
                    btnCheckBalance.Visible = true;
                    btnWithdraw.Visible = true;
                    btnLogout.Visible = true;
                    lblWelcome.Visible = true;
                    lblOptionsText.Visible = true;
                    btnSelectLeft[0].Click += new EventHandler(this.btnWithdraw_Click);
                    btnSelectLeft[1].Click += new EventHandler(this.btnCheckBalance_Click);
                    btnSelectLeft[2].Click += new EventHandler(this.btnLogout_Click);
                    break;
                case ATMState.EnteredAccount:
                    lblKeypad.Text = "";
                    lblKeypad.Visible = true;
                    lblLoginText.Visible = true;
                    lblPinEnter.Visible = true;
                    break;

                case ATMState.DisplayingBalance:
                    lblBalance.Visible = true;
                    btnReturntoMenu.Visible = true;
                    btnSelectRight[2].Click += btnReturntoMenu_Click;
                    break;

                case ATMState.WithdrawingMoney:
                    lblWithdrawInstructions.Visible = true;
                    btnWithdraw10.Visible = true;
                    btnWithdraw50.Visible = true;
                    btnWithdraw500.Visible = true;
                    btnWithdrawCustom.Visible = true;
                    btnReturntoMenu.Visible = true;
                    btnSelectRight[0].Click += btnWithdrawCustom_Click;
                    btnSelectRight[2].Click += btnReturntoMenu_Click;
                    btnSelectLeft[0].Click += btnWithdraw10_Click;
                    btnSelectLeft[1].Click += btnWithdraw50_Click;
                    btnSelectLeft[2].Click += btnWithdraw500_Click;
                    break;

                case ATMState.LoggedOut:
                    activeAccount = null;
                    lblKeypad.Text = "";
                    lblKeypad.Visible = true;
                    lblLoginText.Visible = true;
                    lblAccEnter.Visible = true;
                    break;

                case ATMState.ProcessingWithdrawal:
                    lblProcessWithdraw.Visible = true;
                    break;

                case ATMState.TimedOut:
                    lblTimeout.Visible = true;
                    btnTimeoutLogIn.Visible = true;
                    btnSelectRight[2].Click += BtnTimeoutLogin_Click;
                    break;

            }
        }
    }

    /**
     * Bank Computer Class
     * Contains references to all of the bank account stored in the bank and creates all of the ATMs which are used
     * Used to launch and intialise the ATMs
     */
    public partial class BankComputer : Form
    {
        private Account[] ac = new Account[3]; // create array of accounts
        private System.Windows.Forms.Button btnLaunchATMRace;
        private System.Windows.Forms.Button btnLaunchATMNonRace;
        private bool dataRace = false; // to know if there is a data race or not to use for setup
        private Thread atmThread1, atmThread2, atmThread3, atmThread4; // create threads
        private bool launched = false;
        private ATMForm atmForm1, atmForm2;

        private System.Windows.Forms.Label welcomeText;
        private System.Windows.Forms.Panel colorBlock;
        private System.Windows.Forms.PictureBox logo;
        private System.Windows.Forms.Panel diagonalStripePanel;

        /**
         * This fucntions initilises the 3 accounts and sets up so that the user can use an ATM
         * 
         */
        public BankComputer()
        {
            InitializeComponent();

            InitializeAccounts();

        }


        /**
         * Display the inital screen to the user to allow them to launch ATMS
         */
        private void InitializeComponent()
        {
            // adding controls here for data race and non data race versions so you can start the system with either
            // starting off with 2 ATMs to make it easier to program, we can change later if you guys want to, but i think we need a set number so each is runnign on a manually created thread
            this.Text = "ATM Bank Computer";
            this.btnLaunchATMRace = new System.Windows.Forms.Button();
            this.btnLaunchATMNonRace = new System.Windows.Forms.Button();
            this.Size = new Size(700, 500);

            // create a colored block           
            colorBlock = new Panel { BackColor = Color.DodgerBlue, Dock = DockStyle.Left, Width = this.Width / 2 };
            // add text on top of coloured block
            welcomeText = new Label { Text = "Hello!\nWelcome to\n our bank", ForeColor = Color.White, BackColor = Color.DodgerBlue, Font = new Font("Arial", 30, FontStyle.Regular), BorderStyle = BorderStyle.None, TextAlign = ContentAlignment.MiddleRight, AutoEllipsis = false, Size = new Size(300, 300), Location = new Point(40, 200) };
            logo = new PictureBox { SizeMode = PictureBoxSizeMode.StretchImage, Size = new Size(150, 150), Location = new Point(100, 50) };
            string imageUrl = "https://png.pngtree.com/png-vector/20190225/ourmid/pngtree-concept-banking-logo-png-image_712961.jpg";// get the image from the website
            using (System.Net.WebClient webClient = new System.Net.WebClient())
            {
                byte[] imageData = webClient.DownloadData(imageUrl);
                using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(imageData))
                {
                    logo.Image = Image.FromStream(memoryStream); // add the image to the picture box
                }
            }
            Controls.Add(logo);
            logo.BringToFront();
            Controls.Add(welcomeText);
            welcomeText.BringToFront();
            Controls.Add(colorBlock);


            int stripeWidth = colorBlock.Width;
            int stripeHeight = colorBlock.Height / 3;
            diagonalStripePanel = new Panel { BackColor = Color.Transparent, Size = new Size(stripeWidth, stripeHeight), Location = new Point(0, 0) };
            diagonalStripePanel.Paint += (sender, e) =>
            {
                using (Pen pen = new Pen(Color.DeepSkyBlue, 20)) // adjust the color and width of the line as needed
                {
                    // draw the diagofnal line from top left to bottom right
                    e.Graphics.DrawLine(pen, 0, 0, diagonalStripePanel.Width, diagonalStripePanel.Height);
                }
            };
            colorBlock.Controls.Add(diagonalStripePanel);
            diagonalStripePanel.BringToFront();

            this.btnLaunchATMRace = new System.Windows.Forms.Button { Text = "Launch 2 ATMs (Data Race)", Location = new Point(420, 150), Size = new Size(220, 80), TabIndex = 0, UseVisualStyleBackColor = true, BackColor = Color.MediumBlue, ForeColor = Color.White, Font = new Font("Arial", 18, FontStyle.Regular) };
            this.btnLaunchATMNonRace = new System.Windows.Forms.Button { Text = "Launch 2 ATMs (Non-Data Race)", Location = new Point(420, 250), Size = new Size(220, 80), TabIndex = 0, UseVisualStyleBackColor = true, BackColor = Color.MediumBlue, ForeColor = Color.White, Font = new Font("Arial", 18, FontStyle.Regular) };
            this.btnLaunchATMRace.Click += new EventHandler(this.BtnLaunchATMRace_Click);
            this.btnLaunchATMNonRace.Click += new EventHandler(this.BtnLaunchATMNonRace_Click);
            Controls.Add(this.btnLaunchATMRace);
            Controls.Add(this.btnLaunchATMNonRace);
        }

        /**
         * Initialise threads for the two ATMs to run on and set up for a data race version
         */
        private void BtnLaunchATMRace_Click(object sender, EventArgs e)
        {
            // run two separate ATMs on two different threads 
            dataRace = true;
            atmThread1 = new Thread(new ThreadStart(this.RunATM1));
            atmThread1.Start();

            atmThread2 = new Thread(new ThreadStart(this.RunATM2));
            atmThread2.Start();

            // close the starting form once ATMs have been launched
            this.Close();

        }

        /**
         * Initialise threads for the two ATMs to run on and set up for a non-data race version
         */
        private void BtnLaunchATMNonRace_Click(object sender, EventArgs e)
        {
            dataRace = false;

            atmThread1 = new Thread(new ThreadStart(this.RunATM1));
            //atmThread1.IsBackground = true; if we want the thread all threads to close after main thread closure.
            atmThread1.Start();

            atmThread2 = new Thread(new ThreadStart(this.RunATM2));
            atmThread2.Start();

            // close the starting form once ATMs have been launched
            this.Close();
        }

        /**
         * Create and run first ATM form
         */
        private void RunATM1()
        {
            atmForm1 = new ATMForm(ac, dataRace);
            atmForm1.Icon = Properties.Resources.atmIcon;
            atmForm1.Size = new Size(650, 700);
            atmForm1.StartPosition = FormStartPosition.Manual;
            atmForm1.Location = new Point(0, 0);
            //atmForm1.
            if (dataRace == true)
            {
                atmForm1.Text = "ATM 1 (data race)";
            }
            else
            {
                atmForm1.Text = "ATM 1 (non-data race)";
            }
            Application.Run(atmForm1);
        }

        /**
         * Create and run second ATM form
         */
        private void RunATM2()
        {
            atmForm2 = new ATMForm(ac, dataRace);
            atmForm2.Icon = Properties.Resources.atmIcon;
            atmForm2.Size = new Size(650, 700);
            atmForm2.StartPosition = FormStartPosition.Manual;
            atmForm2.Location = new Point(640, 0);
            if (dataRace == true)
            {
                atmForm2.Text = "ATM 2 (data race)";
            }
            else
            {
                atmForm2.Text = "ATM 2 (non-data race)";
            }
            Application.Run(atmForm2);
        }

        /**
         * Set up the bank accounts with their details ready for use
         */
        private void InitializeAccounts()
        {
            // set up accounts
            ac[0] = new Account(300, 1111, 111111);
            ac[1] = new Account(750, 2222, 222222);
            ac[2] = new Account(3000, 3333, 333333);
        }


        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BankComputer());
        }

    }

    /**
     * The Account class contains all of the information necessary for a bank account
     */
    public class Account
    {
        //the attributes for the account
        private int balance;
        private int pin;
        private int pinAttempts;
        private int accountNum;
        private static Semaphore balanceSemaphore;
        private bool locked;

        /**
         * a constructor that takes initial values for each of the attributes (balance, pin, accountNumber)
         * sets locked and number of pin attempts to default values initially
         */
        public Account(int balance, int pin, int accountNum)
        {
            this.balance = balance;
            this.pin = pin;
            this.accountNum = accountNum;
            this.locked = false;
            this.pinAttempts = 0;
            // set up semaphore to only allow one concurrent entry
            balanceSemaphore = new Semaphore(1, 2);
        }

        /**
         * Get the balance of an account
         */
        public int getBalance()
        {
            return balance;
        }

        /**
         * Set the balance of an account
         * newBalance - value to set the balance to
         */
        public void setBalance(int newBalance)
        {
            this.balance = newBalance;
        }

        /**
         * Get the value of whether an account is locked or not
         */
        public bool getLocked()
        {
            return locked;
        }

        /**
         * Set the value of whether an account is locked or not
         * newLocked - boolean value of whether the account is locked 
         */
        public void setLocked(bool newLocked)
        {
            this.locked = newLocked;
        }

        /**
         * Get the number of wrong pin attempts registered with the account
         */
        public int getPinAttempts()
        {
            return pinAttempts;
        }

        /**
         * Set the number of wrong pin attempts registered with the account to a given value 
         * pinAttempt - value to set the number of pin attempts to
         */
        public void setPinAttempts(int pinAttempt)
        {
            this.pinAttempts = pinAttempt;
        }

        /**
         * Decrement the balance of the account, first checking if there are sufficient funds to do so
         * amount - the amount to decrement the balance by
         * dataRace - boolean to know whether the ATM is in the data race or non data race version
         * return true if transaction was successful, false if there were unsufficient funds
         */
        public Boolean decrementBalance(int amount, bool dataRace)
        {
            if (this.balance > amount)
            {
                if (dataRace == true) // data race version
                {
                    // decrement the balance, then wait before updating it to the account
                    int newBalance = balance - amount;
                    Thread.Sleep(3000); // artificial delay for data race condition demonstration
                                        // using thread.sleep blocks current thread for a specific time
                                        // then update the amount to the main bank account balance
                    balance = newBalance;
                    Console.WriteLine("Balance: " + balance);
                    return true;
                }
                else // non-data race version
                {
                    // decrement the balance, then wait before updating it to the account
                    balanceSemaphore.WaitOne();
                    Console.WriteLine("Thread entering balance semaphore");
                    int newBalance = balance - amount;
                    Thread.Sleep(3000); // artificial delay to increase chances of data race condition demonstration
                                        // using thread.sleep blocks current thread for a specific time
                                        // then update the amount to the main bank account balance
                    balance = newBalance;
                    Console.WriteLine("Balance: " + balance);
                    balanceSemaphore.Release();
                    Console.WriteLine("Thread released balance semaphore");
                    return true;
                }


            }
            else
            {
                return false;
            }
        }

        /**
         * Check the entered pin against the pin stored in the account
         * pinEntered - the entered pin to check against the stored pin
         */
        public Boolean checkPin(int pinEntered)
        {
            if (pinEntered == pin)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /**
         * Get the account number
         */
        public int GetAccountNum()
        {
            return accountNum;
        }
    }
}


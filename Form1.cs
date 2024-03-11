using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

namespace ATM_simulator
{




    /**
     * Main ATM class which is a form
     */
    public partial class ATMForm : Form
    {
        private System.Windows.Forms.Button btnWithdraw;
        private System.Windows.Forms.Button btnCheckBalance;
        private System.Windows.Forms.Button btnLogout;
        private Label lblBalance;
        private System.Windows.Forms.Button btnReturntoMenu;
        private Label lblWithdrawInstructions;
        private ATMState currentState = ATMState.LoggedOut;
        private System.Windows.Forms.Button btnWithdraw10;
        private System.Windows.Forms.Button btnWithdraw50;
        private System.Windows.Forms.Button btnWithdraw500;
        private System.Windows.Forms.Button btnWithdrawCustom; // set up variables
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.TextBox txtAccNum;
        private System.Windows.Forms.TextBox txtPin;
        private System.Windows.Forms.Label loginText;
        private System.Windows.Forms.Label welcomeText;
        private System.Windows.Forms.Label optionsText;
        private System.Windows.Forms.Panel colorBlock;
        private System.Windows.Forms.PictureBox logo;
        private System.Windows.Forms.Panel diagonalStripePanel;

        private Label accPinSymbol;
        private Label accNumSymbol;
        private System.Windows.Forms.Button btnRaceConditionCheck;


        // local reference to array of accounts
        private Account[] ac;

        //this is a referance to the account that is being used
        private Account activeAccount = null;

        // the atm constructor takes an array of account objects as a reference
        //this also has data for the background and other preLogin elements
        public ATMForm(Account[] ac)
        {
            InitializeComponent();
            InitializeWithdrawal();
            this.ac = ac;


            // Set background color to blue
            this.BackColor = Color.White;

            // Increase window size 
            this.Size = new Size(900, 600);

            initialiseLogin();
        }

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

        enum ATMState
        {
            LoggedIn,
            LoggedOut,
            DisplayingBalance,
            WithdrawingMoney

        }

        private void InitializeWithdrawal()
        {
            optionsText = new Label();
            optionsText.Text = "Please select what\nyou would like to do";
            optionsText.ForeColor = Color.White;
            optionsText.BackColor = Color.DodgerBlue;
            optionsText.Font = new Font("Arial", 30, FontStyle.Regular);
            optionsText.BorderStyle = BorderStyle.None;
            optionsText.AutoSize = true;
            optionsText.AutoEllipsis = false;
            optionsText.MaximumSize = new Size(250, 300);
            optionsText.Location = new Point(40, 200); ;
            this.Controls.Add(optionsText);
            optionsText.BringToFront();

            btnWithdraw = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 24, FontStyle.Regular), Text = "Withdraw", Visible = false, Location = new Point(450, 100), Size = new Size(250, 80) };
            btnCheckBalance = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 24, FontStyle.Regular), Text = "Check Balance", Visible = false, Location = new Point(450, 200), Size = new Size(250, 80) };
            btnLogout = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 24, FontStyle.Regular), Text = "Logout", Visible = false, Location = new Point(450, 300), Size = new Size(250, 80) };

            lblBalance = new Label { Location = new Point(550, 230), Size = new Size(217, 30), Visible = false };

            btnReturntoMenu = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 12, FontStyle.Regular), Text = "Return to Account Menu", Size = new Size(150, 30), Location = new Point(325, 280), Visible = false };

            lblWithdrawInstructions = new Label { Text = "Choose your withdrawal amount below: ", BorderStyle = BorderStyle.None, Location = new Point(500, 100), Size = new Size(200, 30), Visible = false };
            btnWithdraw10 = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 24, FontStyle.Regular), Text = "£10", Location = new Point(550, 150), Size = new Size(100, 50), Visible = false };
            btnWithdraw50 = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 24, FontStyle.Regular), Text = "£50", Location = new Point(550, 200), Size = new Size(100, 50), Visible = false };
            btnWithdraw500 = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 24, FontStyle.Regular), Text = "£500", Location = new Point(550, 250), Size = new Size(100, 50), Visible = false };
            btnWithdrawCustom = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 18, FontStyle.Regular), Text = "Custom Amount", Location = new Point(500, 300), Size = new Size(200, 60), Visible = false };

            btnRaceConditionCheck = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Text = "Test Race Condition", Visible = false, Location = new Point(325, 230), Size = new Size(150, 30), Font = new Font("Arial", 12, FontStyle.Italic) };

            btnWithdraw.Click += new EventHandler(this.btnWithdraw_Click);
            btnCheckBalance.Click += new EventHandler(this.btnCheckBalance_Click);
            btnLogout.Click += new EventHandler(this.btnLogout_Click);

            btnReturntoMenu.Click += new EventHandler(this.btnReturntoMenu_Click);
            btnWithdraw10.Click += new EventHandler(this.btnWithdraw10_Click);
            btnWithdraw50.Click += new EventHandler(this.btnWithdraw50_Click);
            btnWithdraw500.Click += new EventHandler(this.btnWithdraw500_Click);
            btnWithdrawCustom.Click += new EventHandler(this.btnWithdrawCustom_Click);

            btnRaceConditionCheck.Click += new EventHandler(this.btnRaceConditionCheck_Click);



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
            Controls.Add(btnRaceConditionCheck);

        }

        private void btnRaceConditionCheck_Click(object sender, EventArgs e)
        {
            concurrentWithdrawals(100);
        }

        public void initialiseLogin()
        {

            // Create a colored block
            colorBlock = new Panel();
            colorBlock.BackColor = Color.DodgerBlue;
            colorBlock.Dock = DockStyle.Left;
            colorBlock.Width = this.Width / 3;
            this.Controls.Add(colorBlock);


            // add text on top of the coloured block
            welcomeText = new Label();
            welcomeText.Text = "Hello!\nWelcome to\n our bank ATM";
            welcomeText.ForeColor = Color.White;
            welcomeText.BackColor = Color.DodgerBlue;
            welcomeText.Font = new Font("Arial", 30, FontStyle.Regular);
            welcomeText.BorderStyle = BorderStyle.None;
            welcomeText.AutoSize = false; // Set to false to prevent word truncation
            welcomeText.TextAlign = ContentAlignment.MiddleRight;
            welcomeText.AutoEllipsis = false;
            welcomeText.Size = new Size(250, 300); // Set size of the label
            welcomeText.Location = new Point(40, 200);
            this.Controls.Add(welcomeText);
            welcomeText.BringToFront();

            logo = new PictureBox();
            //logo.SizeMode = PictureBoxSizeMode.AutoSize;
            logo.SizeMode = PictureBoxSizeMode.StretchImage;
            logo.Size = new Size(150, 150);
            logo.Location = new Point(75, 50);
            string imageUrl = "https://png.pngtree.com/png-vector/20190225/ourmid/pngtree-concept-banking-logo-png-image_712961.jpg";// get the image from the website
            using (System.Net.WebClient webClient = new System.Net.WebClient())
            {
                byte[] imageData = webClient.DownloadData(imageUrl);
                using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(imageData))
                {
                    logo.Image = Image.FromStream(memoryStream); // add the image to the picture box
                }
            }
            this.Controls.Add(logo);
            logo.BringToFront();

            // Create a panel for the diagonal stripe
            int stripeWidth = colorBlock.Width; // Adjust the width of the stripe as needed
            int stripeHeight = colorBlock.Height / 4; // Adjust the height of the stripe as needed
            diagonalStripePanel = new Panel();
            diagonalStripePanel.BackColor = Color.Transparent; // Set the background color to transparent
            diagonalStripePanel.Size = new Size(stripeWidth, stripeHeight); // Set the size of the stripe
            diagonalStripePanel.Location = new Point(0, 0); // Set the location of the diagonal stripe to the top left of the blue block
            diagonalStripePanel.Paint += (sender, e) =>// Override the OnPaint event to draw the diagonal stripe
            {
                using (Pen pen = new Pen(Color.DeepSkyBlue, 20)) // Adjust the color and width of the line as needed
                {
                    // Draw the diagonal line from top left to bottom right
                    e.Graphics.DrawLine(pen, 0, 0, diagonalStripePanel.Width, diagonalStripePanel.Height);
                }
            };
            colorBlock.Controls.Add(diagonalStripePanel);
            diagonalStripePanel.BringToFront();


            // add text
            loginText = new Label();
            loginText.Text = "Login to your account to use our bank";
            loginText.ForeColor = Color.DodgerBlue;
            loginText.BackColor = Color.White;
            loginText.Font = new Font("Arial", 20, FontStyle.Regular);
            loginText.BorderStyle = BorderStyle.None;
            loginText.AutoSize = true;
            loginText.AutoEllipsis = false;
            loginText.MaximumSize = new Size(500, 900);
            loginText.Location = new Point(350, 150);
            this.Controls.Add(loginText);
            loginText.BringToFront();

            // Initialize Confirm button
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnConfirm.Text = "Login";
            this.btnConfirm.ForeColor = Color.White;
            this.btnConfirm.FlatStyle = FlatStyle.Popup;
            this.btnConfirm.Font = new Font("Arial", 12, FontStyle.Regular);
            this.btnConfirm.Location = new Point(350, 320);
            this.btnConfirm.Size = new Size(150, 60);
            this.btnConfirm.BackColor = Color.DodgerBlue;
            this.btnConfirm.Click += new EventHandler(this.btnConfirm_Click); // Subscribe to Click event

            // Add Confirm button to form's controls
            this.Controls.Add(this.btnConfirm);


            // Initialize and position the text box for acc Num
            txtAccNum = new System.Windows.Forms.TextBox();
            txtAccNum.Location = new Point(390, 200);
            txtAccNum.Size = new Size(350, 40);
            txtAccNum.Font = new Font("Arial", 26, FontStyle.Regular);
            txtAccNum.BorderStyle = BorderStyle.None;
            this.Controls.Add(txtAccNum);

            //display a single character next to the account number box
            accNumSymbol = new Label();
            accNumSymbol.Text = "🔑";
            accNumSymbol.Font = new Font("Arial", 22, FontStyle.Regular);
            accNumSymbol.Location = new Point(350, 200);
            accNumSymbol.Size = new Size(40, 40);
            accNumSymbol.BackColor = Color.White;
            this.Controls.Add(accNumSymbol);

            // Initialize and position the text box for acc Pin
            txtPin = new System.Windows.Forms.TextBox();
            txtPin.Location = new Point(390, 250);
            txtPin.Size = new Size(350, 40);
            txtPin.Font = new Font("Arial", 26, FontStyle.Regular);
            txtPin.BorderStyle = BorderStyle.None;
            this.Controls.Add(txtPin);

            //display a single character next to the pin number box
            accPinSymbol = new Label();
            accPinSymbol.Text = "🔓";
            accPinSymbol.Font = new Font("Arial", 22, FontStyle.Regular);
            accPinSymbol.Location = new Point(350, 250);
            accPinSymbol.Size = new Size(40, 40);
            accPinSymbol.BackColor = Color.White;
            this.Controls.Add(accPinSymbol);

        }

        private void btnWithdrawCustom_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter a withdrawal amount:", "Custom Withdrawal", "0", -1, -1);
            if (int.TryParse(input, out int customAmount) && customAmount > 0)
            {
                doWithdrawal(customAmount);
            }
            else
            {
                MessageBox.Show("Insufficient funds ");
            }

        }

        private void btnWithdraw50_Click(object sender, EventArgs e)
        {
            doWithdrawal(50);
        }

        private void btnWithdraw500_Click(object sender, EventArgs e)
        {
            doWithdrawal(500);
        }

        private void btnWithdraw10_Click(object sender, EventArgs e)
        {
            doWithdrawal(10);
        }

        private void btnReturntoMenu_Click(object sender, EventArgs e)
        {
            currentState = ATMState.LoggedIn;
            updateUI(currentState);
        }


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
            btnRaceConditionCheck.Visible = false;


            //Design objects 
            txtAccNum.Visible = false;
            txtPin.Visible = false;
            btnConfirm.Visible = false;
            accPinSymbol.Visible = false;
            accNumSymbol.Visible = false;
            colorBlock.Visible = true;
            welcomeText.Visible = false;
            logo.Visible = false;
            loginText.Visible = false;
            diagonalStripePanel.Visible = false;
            optionsText.Visible = false;
        }



        // im not sure what he meant about
        // "change the operation so that rather than exiting when a person takes
        // their card(option 3), the system goes back to the beginning
        private void btnLogout_Click(object sender, EventArgs e)
        {

            currentState = ATMState.LoggedOut;
            updateUI(currentState);
        }

        private void btnCheckBalance_Click(object sender, EventArgs e)
        {
            currentState = ATMState.DisplayingBalance;
            updateUI(currentState);

            if (this.activeAccount != null)
            {
                this.lblBalance.Text = " your current balance is : " + activeAccount.getBalance();
            }
        }

        private void btnWithdraw_Click(object sender, EventArgs e)
        {
            currentState = ATMState.WithdrawingMoney;
            updateUI(currentState);
        }

        private void concurrentWithdrawals(int withdrawalAmount)
        {
            // creating two threads and decremeting balance of the active account
            var atm1Thread = new Thread(() => activeAccount?.decrementBalance(withdrawalAmount));
            var atm2Thread = new Thread(() => activeAccount?.decrementBalance(withdrawalAmount));

            atm1Thread.Start();
            atm2Thread.Start();

            atm1Thread.Join();
            atm2Thread.Join();

            // invoking method on UI thread using MethodInvoker https://stackoverflow.com/questions/36471563/c-sharp-this-invokemethodinvokerdelegate
            this.Invoke((MethodInvoker)delegate
            {
                MessageBox.Show($"final balance after concurrent withdrawals: {activeAccount.getBalance()}");
            });
        }

        private void doWithdrawal(int amount)
        {
            if (activeAccount != null && activeAccount.decrementBalance(amount))
            {
                MessageBox.Show($"Withdrawal successful, New balance : £{activeAccount.getBalance()} ");
            }
            else
            {
                MessageBox.Show("Insufficient funds");
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtAccNum.Text) || string.IsNullOrWhiteSpace(txtPin.Text))
            {
                MessageBox.Show("Enter your account number and PIN before pressing confirm");
                return;
            }

            if (!int.TryParse(txtAccNum.Text, out int accountNumber) || accountNumber <= 0)
            {
                MessageBox.Show("Please enter a valid account number.");
                return;
            }

            if (!int.TryParse(txtPin.Text, out int pin) || pin <= 0)
            {
                MessageBox.Show("Please enter a valid PIN.");
                return;
            }

            //ATM atm = new ATM();
            activeAccount = findAccount(int.Parse(txtAccNum.Text));
            if (activeAccount != null)
            {
                if (activeAccount.checkPin(int.Parse(txtPin.Text)))
                {
                    currentState = ATMState.LoggedIn;
                    updateUI(currentState);
                }
                else
                {
                    MessageBox.Show("Invalid PIN");
                }
            }
            else
            {
                MessageBox.Show("Account not found");
            }
        }

        // to make UI easier to control 
        private void updateUI(ATMState state)
        {

            resetUI();


            switch (state)
            {
                case ATMState.LoggedIn:
                    btnCheckBalance.Visible = true;
                    btnWithdraw.Visible = true;
                    btnLogout.Visible = true;
                    optionsText.Visible = true;
                    break;
                case ATMState.DisplayingBalance:

                    lblBalance.Visible = true;
                    btnReturntoMenu.Visible = true;
                    break;
                case ATMState.WithdrawingMoney:
                    lblWithdrawInstructions.Visible = true;
                    btnWithdraw10.Visible = true;
                    btnWithdraw50.Visible = true;
                    btnWithdraw500.Visible = true;
                    btnWithdrawCustom.Visible = true;
                    btnReturntoMenu.Visible = true;
                    btnRaceConditionCheck.Visible = true;
                    optionsText.Visible = true;
                    break;
                case ATMState.LoggedOut:

                    txtAccNum.Visible = true;
                    txtPin.Visible = true;
                    btnConfirm.Visible = true;
                    activeAccount = null;
                    txtAccNum.Text = "";
                    txtPin.Text = "";
                    txtAccNum.Visible = true;
                    txtPin.Visible = true;
                    accPinSymbol.Visible = true;
                    accNumSymbol.Visible = true;
                    colorBlock.Visible = true;
                    welcomeText.Visible = true;
                    logo.Visible = true;
                    loginText.Visible = true;
                    diagonalStripePanel.Visible = true;
                    optionsText.Visible = false;
                    break;

            }
        }
    }


    public partial class BankComputer : Form
    {
        private Account[] ac = new Account[3]; // create array of accounts
        // create atm objects
        private ATMForm atm1, atm2;
        private Thread ATM1_t, ATM2_t; // create threads
        private System.Windows.Forms.Button btnLaunchATM;



        /*
         * This fucntions initilises the 3 accounts 
         * and instanciates the ATM class passing a referance to the account information
         * 
         */
        public BankComputer()
        {
            InitializeComponent();

            InitializeAccounts();

            // this runs one ATM - need to run a second one
            //Application.Run(new ATMForm(ac));

            /*
            // run two different ATM Windows
            ATM1_t = new Thread(new ThreadStart(ThreadProc));
            ATM1_t.Start();
            ATM2_t = new Thread(new ThreadStart(ThreadProc));
            ATM2_t.Start();
            */

        }

        private void InitializeComponent()
        {
            this.btnLaunchATM = new System.Windows.Forms.Button();


            this.btnLaunchATM = new System.Windows.Forms.Button { Text = "Launch ATM", Location = new Point(10, 10), Size = new Size(100, 30), TabIndex = 0, UseVisualStyleBackColor = true };
            this.btnLaunchATM.Click += new EventHandler(this.BtnLaunchATM_Click);
            Controls.Add(this.btnLaunchATM);


        }


        private void BtnLaunchATM_Click(object sender, EventArgs e)
        {
            Thread atmThread = new Thread(new ThreadStart(this.RunATM));
            //  atmThread.IsBackground = true; if we want the thread all threads to close after main thread closure.
            atmThread.Start();
        }

        private void RunATM()
        {
            ATMForm atmForm = new ATMForm(ac);
            Application.Run(atmForm);
        }
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



        // got this method from this webiste - https://stackoverflow.com/questions/9856596/multiple-forms-in-separate-threads
        /*
        private void ThreadProc()
        {
            var frm = new ATMForm(ac);
            frm.ShowDialog();
        }

        */

        /*
        static void Main(string[] args)
        {
            new BankComputer();
        }
        */


    }

    public class Account
    {
        //the attributes for the account
        private int balance;
        private int pin;
        private int accountNum;

        // a constructor that takes initial values for each of the attributes (balance, pin, accountNumber)
        public Account(int balance, int pin, int accountNum)
        {
            this.balance = balance;
            this.pin = pin;
            this.accountNum = accountNum;
        }

        //getter and setter functions for balance
        public int getBalance()
        {
            return balance;
        }
        public void setBalance(int newBalance)
        {
            this.balance = newBalance;
        }

        /**
         * Decrement the balance of the account, first checking if there are sufficient funds to do so
         * amount - the amount to decrement the balance by
         * return true if transaction was successful, false if there were unsufficient funds
         */
        public Boolean decrementBalance(int amount)
        {
            if (this.balance > amount)
            {
                Thread.Sleep(2000); // artificial delay to increase chances of data race condition demonstration
                                    // using thread.sleep blocks current thread for a specific time
                balance -= amount;
                return true;
            }
            else
            {
                return false;
            }
        }

        /**
         * Check the entered pin against the pin stored in the account
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

        // get the account number
        public int GetAccountNum()
        {
            return accountNum;
        }
    }
}


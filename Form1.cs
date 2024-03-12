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


        // private System.Windows.Forms.Button btnRaceConditionCheck;


        // local reference to array of accounts
        private Account[] ac;
        private bool dataRace;

        //this is a referance to the account that is being used
        private Account activeAccount = null;

        // the atm constructor takes an array of account objects as a reference
        // also takes a boolean value which tells it whether it is in a data race or not
        public ATMForm(Account[] ac, bool dataRace)
        {
            InitializeComponent();
            InitializeWithdrawal();
            this.ac = ac;
            this.dataRace = dataRace;

            this.BackColor = Color.White;
            this.Size = new Size(900, 600);
            InitializeLogin();


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
            

            optionsText = new Label { Text = "Please select an\noption listed", ForeColor = Color.White, BackColor = Color.DodgerBlue, AutoEllipsis = false, AutoSize = true, BorderStyle = BorderStyle.None, Font = new Font("Arial", 30, FontStyle.Regular), MaximumSize = new Size(220, 300), Location = new Point(40, 200), Visible = false};
            Controls.Add(optionsText);
            optionsText.BringToFront();

            btnWithdraw = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 24, FontStyle.Regular), Text = "Withdraw", Visible = false, Location = new Point(330, 200), Size = new Size(250, 80) };
            btnCheckBalance = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 24, FontStyle.Regular), Text = "Check Balance", Visible = false, Location = new Point(330, 300), Size = new Size(250, 80) };
            btnLogout = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 24, FontStyle.Regular), Text = "Logout", Visible = false, Location = new Point(330, 400), Size = new Size(250, 80) };
            lblBalance = new Label { Location = new Point(400, 230), Size = new Size(217, 60), Visible = false, Font = new Font("Arial", 16, FontStyle.Regular) };
            btnReturntoMenu = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 12, FontStyle.Regular), Text = "Return to Account Menu", Size = new Size(150, 30), Location = new Point(400, 450), Visible = false };
            lblWithdrawInstructions = new Label { Text = "Choose your withdrawal \namount below: ", BorderStyle = BorderStyle.None, Location = new Point(300, 90), Size = new Size(400, 60), Visible = false, Font = new Font("Arial", 16, FontStyle.Regular) };

            btnWithdraw10 = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 24, FontStyle.Regular), Text = "£10", Location = new Point(420, 150), Size = new Size(100, 50), Visible = false };
            btnWithdraw50 = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 24, FontStyle.Regular), Text = "£50", Location = new Point(420, 210), Size = new Size(100, 50), Visible = false };
            btnWithdraw500 = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 24, FontStyle.Regular), Text = "£500", Location = new Point(420, 270), Size = new Size(100, 50), Visible = false };
            btnWithdrawCustom = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 18, FontStyle.Regular), Text = "Custom Amount", Location = new Point(370, 330), Size = new Size(200, 60), Visible = false };

            //btnRaceConditionCheck = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Text = "Test Race Condition", Visible = false, Location = new Point(325, 230), Size = new Size(150, 30), Font = new Font("Arial", 12, FontStyle.Italic) };


            btnWithdraw.Click += new EventHandler(this.btnWithdraw_Click);
            btnCheckBalance.Click += new EventHandler(this.btnCheckBalance_Click);
            btnLogout.Click += new EventHandler(this.btnLogout_Click);
            btnReturntoMenu.Click += new EventHandler(this.btnReturntoMenu_Click);


            btnWithdraw10.Click += new EventHandler(this.btnWithdraw10_Click);
            btnWithdraw50.Click += new EventHandler(this.btnWithdraw50_Click);
            btnWithdraw500.Click += new EventHandler(this.btnWithdraw500_Click);
            btnWithdrawCustom.Click += new EventHandler(this.btnWithdrawCustom_Click);

            //btnRaceConditionCheck.Click += new EventHandler(this.btnRaceConditionCheck_Click);

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
            // Controls.Add(btnRaceConditionCheck);

        }

        private void InitializeLogin()
        {
            // create a colored block           
            colorBlock = new Panel { BackColor = Color.DodgerBlue, Dock = DockStyle.Left, Width = this.Width / 4 + 50 };
            // add text on top of coloured block
            welcomeText = new Label { Text = "Hello!\nWelcome to\n our bank ATM", ForeColor = Color.White, BackColor = Color.DodgerBlue, Font = new Font("Arial", 30, FontStyle.Regular), BorderStyle = BorderStyle.None, TextAlign = ContentAlignment.MiddleRight, AutoEllipsis = false, Size = new Size(230, 300), Location = new Point(40, 200) };
            logo = new PictureBox { SizeMode = PictureBoxSizeMode.StretchImage, Size = new Size(150, 150), Location = new Point(60, 50) };
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
            int stripeHeight = colorBlock.Height / 4;
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

            //add text
            loginText = new Label { Text = "Login to your account to use our bank", ForeColor = Color.DodgerBlue, BackColor = Color.White, Font = new Font("Arial", 20, FontStyle.Regular), BorderStyle = BorderStyle.None, AutoSize = true, AutoEllipsis = false, MaximumSize = new Size(300, 900), Location = new Point(300, 100) };
            Controls.Add(loginText);
            loginText.BringToFront();


            // confirm button 
            btnConfirm = new System.Windows.Forms.Button { Text = "Login", ForeColor = Color.White, FlatStyle = FlatStyle.Popup, Font = new Font("Arial", 12, FontStyle.Regular), Location = new Point(350, 320), Size = new Size(150, 60), BackColor = Color.DodgerBlue };
            btnConfirm.Click += new EventHandler(this.btnConfirm_Click);
            Controls.Add(this.btnConfirm);

            //text box for account number
            txtAccNum = new System.Windows.Forms.TextBox { Location = new Point(390, 200), Size = new Size(350, 40), Font = new Font("Arial", 26, FontStyle.Regular), BorderStyle = BorderStyle.None, };
            Controls.Add(txtAccNum);

            //display a single character next to the account number box
            accNumSymbol = new Label { Text = "🔑", Font = new Font("Arial", 22, FontStyle.Regular), Location = new Point(350, 200), Size = new Size(40, 40), BackColor = Color.White, };
            Controls.Add(accNumSymbol);

            //text box for acc pin
            txtPin = new System.Windows.Forms.TextBox { Location = new Point(390, 250), Size = new Size(350, 40), Font = new Font("Arial", 26, FontStyle.Regular), BorderStyle = BorderStyle.None, };
            Controls.Add(txtPin);

            //display a single character next to the pin number box
            accPinSymbol = new Label { Text = "🔓", Font = new Font("Arial", 22, FontStyle.Regular), Location = new Point(350, 250), Size = new Size(40, 40), BackColor = Color.White, };
            Controls.Add(accPinSymbol);
        }

        /* i think it should work the user does it manually rather than on a button click - to make it more like how it would actually work
        private void btnRaceConditionCheck_Click(object sender, EventArgs e)
        {
            concurrentWithdrawals(100);
        }*/

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
            // btnRaceConditionCheck.Visible = false;


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
                this.lblBalance.Text = "Your current balance is : " + activeAccount.getBalance();
            }
        }

        private void btnWithdraw_Click(object sender, EventArgs e)
        {
            currentState = ATMState.WithdrawingMoney;
            updateUI(currentState);
        }

        // think this should be done manaully rather than automatically in code
        private void concurrentWithdrawals(int withdrawalAmount)
        {
            // creating two threads and decremeting balance of the active account
            /* var atm1Thread = new Thread(() => activeAccount?.decrementBalance(withdrawalAmount));
             var atm2Thread = new Thread(() => activeAccount?.decrementBalance(withdrawalAmount));

             atm1Thread.Start();
             atm2Thread.Start();

             atm1Thread.Join();
             atm2Thread.Join();

             // invoking method on UI thread using MethodInvoker https://stackoverflow.com/questions/36471563/c-sharp-this-invokemethodinvokerdelegate
             this.Invoke((MethodInvoker)delegate
             {
                 MessageBox.Show($"final balance after concurrent withdrawals: {activeAccount.getBalance()}");
             });*/
        }

        private void doWithdrawal(int amount)
        {
            Thread.Sleep(2000);
            if (activeAccount != null && activeAccount.decrementBalance(amount, dataRace))
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

            activeAccount = findAccount(int.Parse(txtAccNum.Text));
            if (activeAccount != null)
            {
                if (activeAccount.checkPin(int.Parse(txtPin.Text)))
                {
                    currentState = ATMState.LoggedIn;
                    MessageBox.Show("Welcome, " + int.Parse(txtAccNum.Text) + "!");
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
                    //btnRaceConditionCheck.Visible = true;
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
        private System.Windows.Forms.Button btnLaunchATMRace;
        private System.Windows.Forms.Button btnLaunchATMNonRace;
        bool dataRace = false; // to know if there is a data race or not to use for setup
        Thread atmThread1, atmThread2, atmThread3, atmThread4; // create threads


        /*
         * This fucntions initilises the 3 accounts 
         * and instanciates the ATM class passing a referance to the account information
         * 
         */
        public BankComputer()
        {
            InitializeComponent();

            InitializeAccounts();

        }

        private void InitializeComponent()
        {
            // adding controls here for data race and non data race versions so you can start the system with either
            // starting off with 2 ATMs to make it easier to program, we can change later if you guys want to, but i think we need a set number so each is runnign on a manually created thread
            this.Text = "ATM Bank Computer";
            this.btnLaunchATMRace = new System.Windows.Forms.Button();
            this.btnLaunchATMNonRace = new System.Windows.Forms.Button();

            this.btnLaunchATMRace = new System.Windows.Forms.Button { Text = "Launch 2 ATMs (Data Race Version)", Location = new Point(10, 10), Size = new Size(120, 50), TabIndex = 0, UseVisualStyleBackColor = true };
            this.btnLaunchATMNonRace = new System.Windows.Forms.Button { Text = "Launch 2 ATMs (Non-Data Race Version)", Location = new Point(150, 10), Size = new Size(120, 50), TabIndex = 0, UseVisualStyleBackColor = true };
            this.btnLaunchATMRace.Click += new EventHandler(this.BtnLaunchATMRace_Click);
            this.btnLaunchATMNonRace.Click += new EventHandler(this.BtnLaunchATMNonRace_Click);
            Controls.Add(this.btnLaunchATMRace);
            Controls.Add(this.btnLaunchATMNonRace);
        }

        // possibly could have atm creation in some sort of loop but they are done separately for now
        private void BtnLaunchATMRace_Click(object sender, EventArgs e)
        {
            // run two separate ATMs on two different threads - we need to have them running on separate threads from the stater
            // do we want the launch form to close when the ATMs are launched, to mean that you can't be running multiple versions of the program at once?
            dataRace = true;
            atmThread1 = new Thread(new ThreadStart(this.RunATM1));
            //atmThread1.IsBackground = true; if we want the thread all threads to close after main thread closure.
            atmThread1.Start();

            atmThread2 = new Thread(new ThreadStart(this.RunATM2));
            atmThread2.Start();
        }

        // Launch ATMs wihtout a data race
        private void BtnLaunchATMNonRace_Click(object sender, EventArgs e)
        {
            dataRace = false;

            atmThread1 = new Thread(new ThreadStart(this.RunATM1));
            //atmThread1.IsBackground = true; if we want the thread all threads to close after main thread closure.
            atmThread1.Start();

            atmThread2 = new Thread(new ThreadStart(this.RunATM2));
            atmThread2.Start();
        }

        // create first ATM form and run
        private void RunATM1()
        {
            ATMForm atmForm1 = new ATMForm(ac, dataRace);
            atmForm1.Icon = Properties.Resources.atmIcon;
            atmForm1.Size = new Size(650, 700);
            atmForm1.StartPosition = FormStartPosition.Manual;
            atmForm1.Location = new Point(0, 0);
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

        // create second ATM form and run
        private void RunATM2()
        {
            ATMForm atmForm2 = new ATMForm(ac, dataRace);
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

    public class Account
    {
        //the attributes for the account
        private int balance;
        private int pin;
        private int accountNum;
        private static Semaphore balanceSemaphore;

        // a constructor that takes initial values for each of the attributes (balance, pin, accountNumber)
        public Account(int balance, int pin, int accountNum)
        {
            this.balance = balance;
            this.pin = pin;
            this.accountNum = accountNum;
            // set up semaphore to only allow one concurrent entry
            balanceSemaphore = new Semaphore(1, 2);
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
        public Boolean decrementBalance(int amount, bool dataRace)
        {
            if (this.balance > amount)
            {
                if (dataRace == true) // data race version
                {
                    // decrement the balance, then wait before updating it to the account
                    int newBalance = balance - amount;
                    Thread.Sleep(3000); // artificial delay to increase chances of data race condition demonstration
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


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
        private Label accPinSymbol;
        private Label accNumSymbol;



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
            this.BackColor = Color.LightSkyBlue;

            // Increase window size 
            this.Size = new Size(700, 600);

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
            btnWithdraw = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 12, FontStyle.Regular), Text = "Withdraw", Visible = false, Location = new Point(217, 110), Size = new Size(200, 30) };

            btnCheckBalance = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 12, FontStyle.Regular), Text = "Check Balance", Visible = false, Location = new Point(217, 150), Size = new Size(200, 30) };
            btnLogout = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 12, FontStyle.Regular), Text = "Logout", Visible = false, Location = new Point(217, 190), Size = new Size(200, 30) };
            lblBalance = new Label { Location = new Point(217, 230), Size = new Size(217, 30), Visible = false };
            btnReturntoMenu = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 12, FontStyle.Regular), Text = "Return to Account Menu", Size = new Size(150, 30), Location = new Point(10, 280), Visible = false };
            lblWithdrawInstructions = new Label { Text = "Choose your withdrawal amount below: ", Location = new Point(217, 100), Size = new Size(200, 30), Visible = false };

            btnWithdraw10 = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 12, FontStyle.Regular), Text = "£10", Location = new Point(267, 150), Size = new Size(100, 50), Visible = false };
            btnWithdraw50 = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 12, FontStyle.Regular), Text = "£50", Location = new Point(267, 200), Size = new Size(100, 50), Visible = false };
            btnWithdraw500 = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 12, FontStyle.Regular), Text = "£500", Location = new Point(267, 250), Size = new Size(100, 50), Visible = false };
            btnWithdrawCustom = new System.Windows.Forms.Button { ForeColor = Color.White, FlatStyle = FlatStyle.Popup, BackColor = Color.MediumBlue, Font = new Font("Arial", 12, FontStyle.Regular), Text = "Custom Amount", Location = new Point(217, 300), Size = new Size(200, 60), Visible = false };

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

        }

        //this is the code that creates the buttons for the login screen
        public void initialiseLogin()
        {

            // Initialize Confirm button
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnConfirm.Text = "Login";
            this.btnConfirm.ForeColor = Color.White;
            this.btnConfirm.FlatStyle = FlatStyle.Popup;
            this.btnConfirm.Font = new Font("Arial", 12, FontStyle.Regular);
            this.btnConfirm.Location = new Point(250, 300); 
            this.btnConfirm.Size = new Size(200, 70);
            this.btnConfirm.BackColor = Color.MediumBlue;
            this.btnConfirm.Click += new EventHandler(this.btnConfirm_Click); // Subscribe to Click event

            // Add Confirm button to form's controls
            this.Controls.Add(this.btnConfirm);


            // Initialize and position the text box for acc Num
            txtAccNum = new System.Windows.Forms.TextBox();
            txtAccNum.Location = new Point(250, 100);
            txtAccNum.Size = new Size(200, 40);
            txtAccNum.Font = new Font("Arial", 26, FontStyle.Regular);
            txtAccNum.BorderStyle = BorderStyle.None;
            this.Controls.Add(txtAccNum);

            //display a single character next to the account number box
            accNumSymbol = new Label();
            accNumSymbol.Text = "🔑";
            accNumSymbol.Font = new Font("Arial", 22, FontStyle.Regular);
            accNumSymbol.Location = new Point(210, 100);
            accNumSymbol.Size = new Size(40, 40);
            accNumSymbol.BackColor = Color.White;
            this.Controls.Add(accNumSymbol);

            // Initialize and position the text box for acc Pin
            txtPin = new System.Windows.Forms.TextBox();
            txtPin.Location = new Point(250, 150);
            txtPin.Size = new Size(200, 40);
            txtPin.Font = new Font("Arial", 26, FontStyle.Regular);
            txtPin.BorderStyle = BorderStyle.None;
            this.Controls.Add(txtPin);

            //display a single character next to the pin number box
            accPinSymbol = new Label();
            accPinSymbol.Text = "🔓";
            accPinSymbol.Font = new Font("Arial", 22, FontStyle.Regular);
            accPinSymbol.Location = new Point(210, 150);
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


            //Design objects 
            txtAccNum.Visible = false;
            txtPin.Visible = false;
            btnConfirm.Visible = false;
            accPinSymbol.Visible = false;
            accNumSymbol.Visible = false;

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
                    break;
                case ATMState.LoggedOut:

                    txtAccNum.Visible = true;
                    txtPin.Visible = true;
                    btnConfirm.Visible = true;
                    activeAccount = null;
                    txtAccNum.Text = "";
                    txtPin.Text = "";
                    accPinSymbol.Visible = true;
                    accNumSymbol.Visible = true;

                    break;

            }
        }

        private void txtAccNum_TextChanged(object sender, EventArgs e)
        {

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

        //This is the code that creates the button for (launch ATM)
       private void InitializeComponent()
        {
            this.btnLaunchATM = new System.Windows.Forms.Button();


            this.btnLaunchATM = new System.Windows.Forms.Button { Text = "Launch ATM", Location = new Point(20, 20), Size = new Size(100, 30), TabIndex = 0, UseVisualStyleBackColor = true };
            this.btnLaunchATM.Click += new EventHandler(this.BtnLaunchATM_Click);
            Controls.Add(this.btnLaunchATM);


        }

        //This is the code that handles the button click for launch ATM
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


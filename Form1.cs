using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATM_simulator
{
    enum ATMState
    {
        LoggedIn,
        LoggedOut,
        DisplayingBalance,
        WithdrawingMoney

    }

    public partial class ATMForm : Form
    {
        Account activeAccount;
        Account amount;
        private Button btnWithdraw;
        private Button btnCheckBalance;
        private Button btnLogout;
        private Button btnNewAtm;
        private Label lblBalance;
        private Button btnReturntoMenu;
        private Label lblWithdrawInstructions;
        private ATMState currentState = ATMState.LoggedOut;
        private Button btnWithdraw10;
        private Button btnWithdraw50;
        private Button btnWithdraw500;
        private Button btnWithdrawCustom;



        private void Initializing()
        {
            btnWithdraw = new Button { Text = "Withdraw", Visible = false, Location = new Point(217, 110), Size = new Size(200, 30) };
            btnCheckBalance = new Button { Text = "Check Balance", Visible = false, Location = new Point(217, 150), Size = new Size(200, 30) };
            btnLogout = new Button { Text = "Logout", Visible = false, Location = new Point(217, 190), Size = new Size(200, 30) };
            btnNewAtm = new Button { Text = "New ATM", Visible = false, Location = new Point(217, 270), Size = new Size(200, 30) };
            lblBalance = new Label { Location = new Point(217, 230), Size = new Size(217, 30), Visible = false };
            btnReturntoMenu = new Button { Text = "Return to Account Menu", Size = new Size(150, 30), Location = new Point(10, 280), Visible = false };
            lblWithdrawInstructions = new Label { Text = "Choose your withdrawal amount below: ", Location = new Point(217, 100), Size = new Size(200, 30), Visible = false };

            btnWithdraw10 = new Button { Text = "£10", Location = new Point(267, 150), Size = new Size(100, 50), Visible = false };
            btnWithdraw50 = new Button { Text = "£50", Location = new Point(267, 200), Size = new Size(100, 50), Visible = false };
            btnWithdraw500 = new Button { Text = "£500", Location = new Point(267, 250), Size = new Size(100, 50), Visible = false };
            btnWithdrawCustom = new Button { Text = "Custom Amount", Location = new Point(217, 300), Size = new Size(200, 60), Visible = false };

            btnWithdraw.Click += new EventHandler(this.btnWithdraw_Click);
            btnCheckBalance.Click += new EventHandler(this.btnCheckBalance_Click);
            btnLogout.Click += new EventHandler(this.btnLogout_Click);
            btnNewAtm.Click += new EventHandler(this.btnNewAtm_Click);
            btnReturntoMenu.Click += new EventHandler(this.btnReturntoMenu_Click);
            btnWithdraw10.Click += new EventHandler(this.btnWithdraw10_Click);
            btnWithdraw50.Click += new EventHandler(this.btnWithdraw50_Click);
            btnWithdraw500.Click += new EventHandler(this.btnWithdraw500_Click);
            btnWithdrawCustom.Click += new EventHandler(this.btnWithdrawCustom_Click);



            Controls.Add(lblBalance);
            Controls.Add(btnWithdraw);
            Controls.Add(btnCheckBalance);
            Controls.Add(btnLogout);
            Controls.Add(btnNewAtm);
            Controls.Add(btnReturntoMenu);
            Controls.Add(lblWithdrawInstructions);
            Controls.Add(btnWithdraw10);
            Controls.Add(btnWithdraw50);
            Controls.Add(btnWithdraw500);
            Controls.Add(btnWithdrawCustom);

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
            btnNewAtm.Visible = false;
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
        }
        private void btnNewAtm_Click(object sender, EventArgs e)
        {
            MessageBox.Show("New ATM clicked - still to be implemented.");

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

        public ATMForm()
        {
            InitializeComponent();
            Initializing();

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


            ATM atm = new ATM();
            activeAccount = atm.findAccount(int.Parse(txtAccNum.Text));
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
                    btnNewAtm.Visible = true;
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
                    btnReturntoMenu.Visible=true;
                    break;
                case ATMState.LoggedOut:

                    txtAccNum.Visible = true;
                    txtPin.Visible = true;
                    btnConfirm.Visible = true;
                    activeAccount = null;
                    txtAccNum.Text = "";
                    txtPin.Text="";
                    break;

            }
        }
        
        class ATM
        {
            private Account[] ac = new Account[3];

            public ATM()
            {
                ac[0] = new Account(300, 1111, 111111);
                ac[1] = new Account(750, 2222, 222222);
                ac[2] = new Account(3000, 3333, 333333);

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
        }
    }




    class Account
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
        public int GetAccountNum()
        {
            return accountNum;
        }
    }
}


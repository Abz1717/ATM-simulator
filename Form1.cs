using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATM_simulator
{



    public partial class ATMForm : Form
    {

        private Account activeAccount = null;
        private Button btnWithdraw;
        private Button btnCheckBalance;
        private Button btnLogout;
        private Button btnNewAtm;
        private Label lblBalance;



        private void Initializing()
        {
            btnWithdraw = new Button { Text = "Withdraw", Visible = false, Location = new Point(10, 110), Size = new Size(200, 30) };
            btnCheckBalance = new Button { Text = "Check Balance", Visible = false, Location = new Point(10, 150), Size = new Size(200, 30) };
            btnLogout = new Button { Text = "Logout", Visible = false, Location = new Point(10, 190), Size = new Size(200, 30) };
            btnNewAtm = new Button { Text = "New ATM", Visible = false, Location = new Point(10, 270), Size = new Size(200, 30) };
            lblBalance = new Label { Location = new Point(10, 230), Size = new Size(200, 30), Visible = false };


            btnWithdraw.Click += new EventHandler(this.btnWithdraw_Click);
            btnCheckBalance.Click += new EventHandler(this.btnCheckBalance_Click);
            btnLogout.Click += new EventHandler(this.btnLogout_Click);
            btnNewAtm.Click += new EventHandler(this.btnNewAtm_Click);

            Controls.Add(lblBalance);
            Controls.Add(btnWithdraw);
            Controls.Add(btnCheckBalance);
            Controls.Add(btnLogout);
        }

        private void btnNewAtm_Click(object sender, EventArgs e)
        {
            MessageBox.Show("New ATM Clicked - Functionality to be implemented.");

        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Controls.Clear();
        }

        private void btnCheckBalance_Click(object sender, EventArgs e)
        {
            lblBalance.Visible = true;

            if (this.activeAccount != null)
            {
                this.lblBalance.Text = " your current balance is : " + activeAccount.getBalance();
            }
        }

        private void btnWithdraw_Click(object sender, EventArgs e)
        {

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
                    MenuAfterConfirmation();

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



        private void MenuAfterConfirmation()
        {
            btnWithdraw.Visible = true;
            btnCheckBalance.Visible = true;
            btnLogout.Visible = true;
            btnNewAtm.Visible = true;

            txtAccNum.Visible = false;
            txtPin.Visible = false;
            btnConfirm.Visible = false;

            if (Visible == false)
            {
                txtAccNum.Clear();
                txtPin.Clear();
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


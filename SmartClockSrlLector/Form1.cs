using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartClockSrlLector.Model;

namespace SmartClockSrlLector
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public List<int> Cache = new List<int>() {-1};

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            using (smartclocksrldbEntities db = new smartclocksrldbEntities())
            {
                employees oEmployees = db.employees.Find((int)cmbUsersDevelop.SelectedValue);
                if (oEmployees != null)
                {
                    lblName.Text = oEmployees.name;
                    lblPosition.Text = oEmployees.position.name;
                    if (oEmployees.images != null)
                    {
                        MemoryStream ms = new MemoryStream(oEmployees.images);
                        Image returnImage = Image.FromStream(ms);
                        picImage.Image = returnImage;
                    }
                    if (registerEmployee(oEmployees))
                    {
                        panelFinger.BackColor = Color.ForestGreen;
                        panelInformation.Visible = true;
                        lblError.Visible = false;
                        timerProfile.Start();
                    }
                    else
                    {
                        lblError.Visible = true;
                    }
                }
                else
                {
                    lblError.Visible = true;
                }
            }
                
        }

        private void datehour_Tick(object sender, EventArgs e)
        {
            lblHour.Text = DateTime.Now.ToString("HH:mm");
            lblDate.Text = DateTime.Now.ToString("dddd, MMM dd, yyyy");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using (smartclocksrldbEntities db = new smartclocksrldbEntities())
            {
                var lst = from d in db.employees
                          select d;
                cmbUsersDevelop.DataSource = lst.ToList();
                cmbUsersDevelop.DisplayMember = "id";
                cmbUsersDevelop.ValueMember = "id";
            }
        }

        private void timerProfile_Tick(object sender, EventArgs e)
        {
            panelFinger.BackColor = Color.FromArgb(40,40,40);
            panelInformation.Visible = false;
            lblError.Visible = false;
            timerProfile.Stop();
        }

        private bool registerEmployee(employees e)
        {
            try
            {
                using (smartclocksrldbEntities db = new smartclocksrldbEntities())
                {
                    if (!this.Cache.Contains(e.id))
                    {
                        checkin oCheckin = new checkin
                        {
                            employeeID = e.id,
                            checkDate = DateTime.Now.Date,
                            checkHour = DateTime.Now.ToString("HH:mm:ss"),
                            observations = null
                        };
                        db.checkin.Add(oCheckin);
                        db.SaveChanges();
                        lblStatus.Text = "Check In";
                        lblStatus.ForeColor = Color.LightGreen;
                        this.Cache.Add(e.id);
                        return true;
                    }
                    else
                    {
                        checkout oCheckout = new checkout
                        {
                            employeeID = e.id,
                            checkDate = DateTime.Now.Date,
                            checkHour = DateTime.Now.ToString("HH:mm:ss"),
                            observations = null
                        };
                        db.checkout.Add(oCheckout);
                        db.SaveChanges();
                        lblStatus.Text = "Check Out";
                        lblStatus.ForeColor = Color.LightGreen;
                        this.Cache.Remove(e.id);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message);
                return false;
            }
        }
    }
}

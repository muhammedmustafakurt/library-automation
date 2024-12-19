using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace kutuphaneotomasyonu
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\kurtm\Desktop\Yeni Microsoft Access Database.accdb;";

        private void button1_Click(object sender, EventArgs e)
        {
            string number = textBox1.Text;
            if (string.IsNullOrWhiteSpace(number))
            {
                MessageBox.Show("Lütfen bir numara girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Adi, Fakultesi, Bolumu FROM Ogrenciler WHERE number = @number";
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@number", number);
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string ad = reader["Adi"].ToString();
                                string fakulte = reader["Fakultesi"].ToString();
                                string bolum = reader["Bolumu"].ToString();
                                label1.Text = $"Adı: {ad}, Fakültesi: {fakulte}, Bölümü: {bolum}";
                            }
                            else
                            {
                                label1.Text = "Kayıt bulunamadı.";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string adi = textBox2.Text;
            string number = textBox3.Text;
            string fakultesi = textBox4.Text;
            string bolumu = textBox5.Text;

            if (string.IsNullOrWhiteSpace(adi) || string.IsNullOrWhiteSpace(number) || string.IsNullOrWhiteSpace(fakultesi) || string.IsNullOrWhiteSpace(bolumu))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO Ogrenciler (Adi, [Number], Fakultesi, Bolumu) VALUES (@Adi, @Number, @Fakultesi, @Bolumu)";
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Adi", adi);
                        command.Parameters.AddWithValue("@Number", number);
                        command.Parameters.AddWithValue("@Fakultesi", fakultesi);
                        command.Parameters.AddWithValue("@Bolumu", bolumu);
                        command.ExecuteNonQuery();
                        MessageBox.Show("Kayıt başarıyla eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string studentID = textBox6.Text;
            string bookID = textBox7.Text;

            if (string.IsNullOrWhiteSpace(studentID) || string.IsNullOrWhiteSpace(bookID))
            {
                MessageBox.Show("Lütfen öğrenci ID ve kitap ID girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string checkStockQuery = "SELECT StokAdedi FROM Kitaplar2 WHERE KitapID = @KitapID";
                    using (OleDbCommand checkStockCommand = new OleDbCommand(checkStockQuery, connection))
                    {
                        checkStockCommand.Parameters.AddWithValue("@KitapID", bookID);
                        int stock = (int)checkStockCommand.ExecuteScalar();

                        if (stock <= 0)
                        {
                            MessageBox.Show("Bu kitap stokta yok!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        string updateStockQuery = "UPDATE Kitaplar2 SET StokAdedi = StokAdedi - 1 WHERE KitapID = @KitapID";
                        using (OleDbCommand updateStockCommand = new OleDbCommand(updateStockQuery, connection))
                        {
                            updateStockCommand.Parameters.AddWithValue("@KitapID", bookID);
                            updateStockCommand.ExecuteNonQuery();
                        }

                        string loanDate = DateTime.Now.ToString("yyyy-MM-dd");
                        string returnDate = DateTime.Now.AddDays(14).ToString("yyyy-MM-dd");

                        string query = "INSERT INTO Odunc (KitapID, OgrenciID, OduncTarihi, IadeTarihi) VALUES (@KitapID, @OgrenciID, @OduncTarihi, @IadeTarihi)";
                        using (OleDbCommand command = new OleDbCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@KitapID", bookID);
                            command.Parameters.AddWithValue("@OgrenciID", studentID);
                            command.Parameters.AddWithValue("@OduncTarihi", loanDate);
                            command.Parameters.AddWithValue("@IadeTarihi", returnDate);
                            command.ExecuteNonQuery();
                        }

                        MessageBox.Show("Kitap ödünç alındı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

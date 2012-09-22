using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Collections.Specialized;
using System.IO;

namespace MCForge.Gui
{
    public partial class LavaMapSubmit : Form
    {
        public LavaMapSubmit()
        {
            InitializeComponent();
        }

        private void LavaMapSubmit_Load(object sender, EventArgs e)
        {
            cmbMap.Items.AddRange(Server.lava.Maps.ToArray());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(delegate
            {
                this.Invoke(new MethodInvoker(delegate { this.Enabled = false; }));
                openFileDialog1.Title = "Select Map Screenshot";
                openFileDialog1.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    this.Invoke(new MethodInvoker(delegate { txtImageFile.Text = openFileDialog1.FileName; }));
                this.Invoke(new MethodInvoker(delegate { this.Enabled = true; this.Focus(); }));
            }));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cmbMap.SelectedItem == null || txtImageFile.Text.Trim() == String.Empty || txtLevelName.Text.Trim() == String.Empty || txtMapDesc.Text.Trim() == String.Empty)
            {
                MessageBox.Show(this, "All fields must be filled before the map can be submitted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }


            this.ToggleAllShit(false);

            try
            {
                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

                Dictionary<string, object> formData = new Dictionary<string, object>();
                formData.Add("lvl", new FileInfo(new StringBuilder(Environment.CurrentDirectory).Append(@"\levels\").Append(cmbMap.SelectedItem).Append(".lvl").ToString()));
                formData.Add("properties", new FileInfo(new StringBuilder(Environment.CurrentDirectory).Append(@"\properties\lavasurvival\").Append(cmbMap.SelectedItem).Append(".properties").ToString()));
                formData.Add("image", new FileInfo(txtImageFile.Text));
                formData.Add("name", txtLevelName.Text);
                formData.Add("desc", txtMapDesc.Text);
                byte[] requestData = BuildFormData(formData, boundary, boundaryBytes);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri("http://www.mcforge.net/maps/submit"));
                request.CookieContainer = MCForgeAccount.Cookies;
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.ContentLength = requestData.Length;
                request.Method = "POST";
                request.KeepAlive = true;
                request.Credentials = CredentialCache.DefaultCredentials;
                request.BeginGetRequestStream(new AsyncCallback(delegate(IAsyncResult result)
                {
                    using (Stream stream = ((HttpWebRequest)result.AsyncState).EndGetRequestStream(result))
                    {
                        using (MemoryStream ms = new MemoryStream(requestData))
                        {
                            int count, total = 0;
                            byte[] buffer = new byte[256];
                            while ((count = ms.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                total += count;
                                stream.Write(buffer, 0, count);
                                this.Invoke(new MethodInvoker(delegate { progressBar1.Value = (int)(((float)total / (float)requestData.Length) * 100); }));
                            }
                        }
                    }
                    using (StreamReader reader = new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream()))
                    {
                        string response = reader.ReadToEnd();
                        if (response.IndexOf("The map has been submitted, and is in the approval queue.") != -1)
                        {
                            MessageBox.Show("The map has been submitted, and is in the approval queue.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Invoke(new MethodInvoker(delegate { this.Dispose(); }));
                        }
                        else
                        {
                            MessageBox.Show("Map submission failed, please check all fields and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            this.Invoke(new MethodInvoker(delegate { this.ToggleAllShit(true); this.Focus(); }));
                        }
                    }
                }), request);
            }
            catch (Exception ex)
            {
                Server.ErrorLog(ex);
                MessageBox.Show("There was an error during the upload, see error log for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.ToggleAllShit(true);
                this.Focus();
            }
        }

        private void cmbMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMap.SelectedItem != null)
                txtLevelName.Text = cmbMap.SelectedItem.ToString().Capitalize();
        }

        private byte[] BuildFormData(Dictionary<string, object> data, string boundary, byte[] boundaryBytes) {
            using (MemoryStream ms = new MemoryStream())
            {
                string template = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                string fileTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                foreach (KeyValuePair<string, object> kvp in data)
                {
                    if (kvp.Value == null) continue;
                    ms.Write(boundaryBytes, 0, boundaryBytes.Length);
                    if (kvp.Value.GetType() == typeof(FileInfo))
                    {
                        FileInfo file = (FileInfo)kvp.Value;
                        if (!file.Exists) throw new FileNotFoundException(file.FullName);
                        byte[] header = Encoding.UTF8.GetBytes(string.Format(fileTemplate, kvp.Key, file.Name, file.GetMimeType()));
                        ms.Write(header, 0, header.Length);
                        byte[] bytes = File.ReadAllBytes(file.FullName);
                        ms.Write(bytes, 0, bytes.Length);
                    }
                    else
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(string.Format(template, kvp.Key, kvp.Value));
                        ms.Write(bytes, 0, bytes.Length);
                    }
                }
                byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                ms.Write(trailer, 0, trailer.Length);
                return ms.ToArray();
            }
        }

        private void ToggleAllShit(bool toggle)
        {
            cmbMap.Enabled = toggle;
            button1.Enabled = toggle;
            txtImageFile.Enabled = toggle;
            txtLevelName.Enabled = toggle;
            txtMapDesc.Enabled = toggle;
            button3.Enabled = toggle;
        }

        private void LavaMapSubmit_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!button3.Enabled)
                e.Cancel = true;
        }
    }
}

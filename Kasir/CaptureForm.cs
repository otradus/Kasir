using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Kasir
{
    /* NOTE: This form is a base for the EnrollmentForm and the VerificationForm,
		All changes in the CaptureForm will be reflected in all its derived forms.
	*/
    public partial class CaptureForm : Form, DPFP.Capture.EventHandler
    {
        public static bool verified = false;
        public bool supervisor = false;

        public void Verify(DPFP.Template template)
        {
            Template = template;
            ShowDialog();
        }

        public CaptureForm(bool supervisor1 = false)
        {
            supervisor = supervisor1;
            InitializeComponent();
        }


        protected virtual void Init()
        {
            Capturer = new DPFP.Capture.Capture();                  // Create a capture operation.
            Capturer.EventHandler = this;                           // Subscribe for capturing events.

            this.Text = "Fingerprint Verification";
            Verificator = new DPFP.Verification.Verification();     // Create a fingerprint template verificator
            UpdateStatus(0);
        }

        protected virtual void Process(DPFP.Sample Sample)
        {
            // Draw fingerprint sample image.
            DrawPicture(ConvertSampleToBitmap(Sample));

            // Process the sample and create a feature set for the enrollment purpose.
            DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Verification);

            // Check quality of the sample and start verification if it's good
            // TODO: move to a separate task
            if (features != null)
            {
                // Compare the feature set with our template
                DPFP.Verification.Verification.Result result = new DPFP.Verification.Verification.Result();

                //MY MODIFICATIONS
                if (supervisor == false)
                {
                    string[] files = Directory.GetFiles("C:\\test\\fingerprints");
                    foreach (string fileName in files)
                    {
                        using (FileStream fs = File.OpenRead(fileName))
                        {
                            DPFP.Template template = new DPFP.Template(fs);
                            Verificator.Verify(features, template, ref result);
                            UpdateStatus(result.FARAchieved);
                            if (result.Verified)
                            {
                                Console.WriteLine("The fingerprint was VERIFIED.");
                                verified = true;
                                break;
                            }
                            else
                            {
                                Console.WriteLine("The fingerprint was NOT VERIFIED.");
                            }
                        }
                    }
                    if (verified == false)
                    {
                        MessageBox.Show("Unauthorized");
                        Application.Exit();
                    }
                    else
                    {
                        if (InvokeRequired) // Line #1
                        {
                            this.Invoke(new MethodInvoker(CloseMe));
                            return;
                        }
                    }
                }
                else
                {
                    string[] files = Directory.GetFiles("C:\\test\\fingerprints");
                    foreach (string fileName in files)
                    {
                        using (FileStream fs = File.OpenRead(fileName))
                        {
                            DPFP.Template template = new DPFP.Template(fs);
                            Verificator.Verify(features, template, ref result);
                            UpdateStatus(result.FARAchieved);
                            if (result.Verified)
                            {
                                Console.WriteLine("The fingerprint was VERIFIED.");
                                verified = true;
                                break;
                            }
                            else
                            {
                                Console.WriteLine("The fingerprint was NOT VERIFIED.");
                            }
                        }
                    }
                    if (verified == false)
                    {
                        MessageBox.Show("Unauthorized");
                        Application.Exit();
                    }
                    else
                    {
                        if (InvokeRequired) // Line #1
                        {
                            this.Invoke(new MethodInvoker(CloseMe));
                            //open supervisorform
                            return;
                        }
                    }
                }
            }
        }

        protected void Start()
        {
            Capturer.StartCapture();
            SetPrompt("Using the fingerprint reader, scan your fingerprint.");
        }

        protected void Stop()
        {
            Capturer.StopCapture();
        }

        private DPFP.Template Template;
        private DPFP.Verification.Verification Verificator;

        #region Form Event Handlers:

        private void CaptureForm_Load(object sender, EventArgs e)
        {
            Init();
            Start();                                                // Start capture operation.
        }

        private void CaptureForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Stop();
            if (verified == false)
            {
                Application.Exit();
            }
        }
        #endregion

        #region EventHandler Members:

        public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
        {
            MakeReport("The fingerprint sample was captured.");
            SetPrompt("Scan the same fingerprint again.");
            Process(Sample);
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber)
        {
            MakeReport("The finger was removed from the fingerprint reader.");
        }

        public void OnFingerTouch(object Capture, string ReaderSerialNumber)
        {
            MakeReport("The fingerprint reader was touched.");
        }

        public void OnReaderConnect(object Capture, string ReaderSerialNumber)
        {
            MakeReport("The fingerprint reader was connected.");
        }

        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber)
        {
            MakeReport("The fingerprint reader was disconnected.");
        }

        public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback)
        {
            if (CaptureFeedback == DPFP.Capture.CaptureFeedback.Good)
                MakeReport("The quality of the fingerprint sample is good.");
            else
                MakeReport("The quality of the fingerprint sample is poor.");
        }
        #endregion

        protected Bitmap ConvertSampleToBitmap(DPFP.Sample Sample)
        {
            DPFP.Capture.SampleConversion Convertor = new DPFP.Capture.SampleConversion();  // Create a sample convertor.
            Bitmap bitmap = null;                                                           // TODO: the size doesn't matter
            Convertor.ConvertToPicture(Sample, ref bitmap);                                 // TODO: return bitmap as a result
            return bitmap;
        }

        protected DPFP.FeatureSet ExtractFeatures(DPFP.Sample Sample, DPFP.Processing.DataPurpose Purpose)
        {
            DPFP.Processing.FeatureExtraction Extractor = new DPFP.Processing.FeatureExtraction();  // Create a feature extractor
            DPFP.Capture.CaptureFeedback feedback = DPFP.Capture.CaptureFeedback.None;
            DPFP.FeatureSet features = new DPFP.FeatureSet();
            Extractor.CreateFeatureSet(Sample, Purpose, ref feedback, ref features);            // TODO: return features as a result?
            if (feedback == DPFP.Capture.CaptureFeedback.Good)
                return features;
            else
                return null;
        }

        private void SetStatus(string status)
        {
            this.Invoke(new Function(delegate ()
            {
                StatusLine.Text = status;
            }));
        }

        private void UpdateStatus(int FAR)
        {
            // Show "False accept rate" value
            SetStatus(String.Format("False Accept Rate (FAR) = {0}", FAR));
        }


        protected void SetPrompt(string prompt)
        {
            this.Invoke(new Function(delegate ()
            {
                Prompt.Text = prompt;
            }));
        }
        protected void MakeReport(string message)
        {
            this.Invoke(new Function(delegate ()
            {
                StatusText.AppendText(message + "\r\n");
            }));
        }

        private void DrawPicture(Bitmap bitmap)
        {
            this.Invoke(new Function(delegate ()
            {
                Picture.Image = new Bitmap(bitmap, Picture.Size);   // fit the image into the picture box
            }));
        }

        private DPFP.Capture.Capture Capturer;

        private void ExitApp()
        {
            Application.Exit();
        }

        private void CloseMe()
        {
            Close();
        }
    }

}
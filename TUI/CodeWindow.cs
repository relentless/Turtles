using Microsoft.FSharp.Collections;
using System;
using System.Drawing;
using System.Windows.Forms;
using Turtle;

namespace TUI {
    public partial class CodeWindow : Form {
        private FSharpList<Tuple<Tuple<double, double>, Tuple<double, double>>> _lines;

        public CodeWindow() {
            InitializeComponent();
        }

        private void Go_Click(object sender, EventArgs e) {
            var sourceCode = Source.Text;
            try {
                var commands = Parser.parse(sourceCode);
                var startTurtle = new Interpreter.Turtle(turtleTrailBox.Width / 2.0, turtleTrailBox.Height / 2.0, 0.0);
                _lines = Interpreter.execute(startTurtle, commands);

                turtleTrailBox.Refresh();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void turtleTrailBox_Paint(object sender, PaintEventArgs e) {

            if (_lines == null)
                return;

            var image = new Bitmap(turtleTrailBox.Width, turtleTrailBox.Height);

            foreach( var line in _lines ){
                Graphics.FromImage(image).DrawLine(
                    new Pen(Color.Red,2f), 
                    (float)line.Item1.Item1,
                    (float)line.Item1.Item2,
                    (float)line.Item2.Item1,
                    (float)line.Item2.Item2);
            }

            // flip image because windows y-coordinates are 'backwards'
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);
            e.Graphics.DrawImageUnscaled(image,0,0);
        }

        private void ClearButton_Click(object sender, EventArgs e) {
            Source.Text = "";
            _lines = null;
            turtleTrailBox.Refresh();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == (Keys.Control | Keys.D1)) {
                Source.Text = @"repeat 4
  [right 90 forward 80]";
                return true;
            }
            if (keyData == (Keys.Control | Keys.D2)) {
                Source.Text = @"repeat 10
  [right 36
  repeat 5 [forward 54 right 72]]";
                return true;
            }
            if (keyData == (Keys.Control | Keys.D3)) {
                Source.Text = @"repeat 36
  [right 10 
  repeat 2 [forward 100 right 90]
  repeat 2 [forward 100 right 91]]";
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}

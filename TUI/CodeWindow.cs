using Microsoft.FSharp.Collections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Turtle;

namespace TUI {
    public partial class CodeWindow : Form {
        private FSharpList<Turtle.Interpreter.Line> _lines;

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

        private Dictionary<string, Color> _colours = new Dictionary<string, Color> {
            {"red",Color.Red},
            {"blue", Color.Blue},
            {"green", Color.Green},
            {Turtle.Interpreter.defaultColour, Color.Black}
        };

        private void turtleTrailBox_Paint(object sender, PaintEventArgs e) {

            if (_lines == null)
                return;

            var image = new Bitmap(turtleTrailBox.Width, turtleTrailBox.Height);

            foreach( var line in _lines ){

                var penColour = _colours[line.Colour];

                Graphics.FromImage(image).DrawLine(
                    new Pen(penColour, 2f), 
                    (float)line.StartPoint.X,
                    (float)line.StartPoint.Y,
                    (float)line.EndPoint.X,
                    (float)line.EndPoint.Y);
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
            if (keyData == (Keys.Control | Keys.D4)) {
                Source.Text = @"set-colour(red)
repeat 36
  [rt 10 fd 20 rt 170 fd 20 lt 170]
rt 10 fd 22 rt 90
set-colour(blue)
repeat 36
  [forward 10 right 14]";
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}

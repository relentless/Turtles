using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;
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

            foreach( var line in _lines ){
                e.Graphics.DrawLine(
                    new Pen(Color.Red,2f), 
                    (float)line.Item1.Item1,
                    (float)line.Item1.Item2,
                    (float)line.Item2.Item1,
                    (float)line.Item2.Item2);
            }
        }
    }
}

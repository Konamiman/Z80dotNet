using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Konamiman.NestorMSX.Hardware
{
    public class ConsoleKeyEventSource : IKeyEventSource
    {
        private Task task;

        void Do()
        {
            while(true)
            {
                while (!Console.KeyAvailable) ;
                var key = Console.ReadKey();
                if(KeyPressed!=null)
                    KeyPressed(this, new KeyEventArgs((Keys)key.Key));
            }
        }

        public void Start()
        {
            task = new Task(Do);
            task.Start();
        }

        public void Stop()
        {
        }

        public event EventHandler<KeyEventArgs> KeyPressed;
        public event EventHandler<KeyEventArgs> KeyReleased;
    }
}

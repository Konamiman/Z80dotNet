using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Hardware
{
    public class KeyboardController : IKeyboardController
    {
        private readonly IKeyEventSource eventSource;
        private Dictionary<Keys, KeyMapping> keyMappingsByKeyId;
        private int currentMatrixRow;

        public KeyboardController(IKeyEventSource eventSource, string keyMappingsMatrix)
        {
            this.eventSource = eventSource;
            InitializeKeyMappings(keyMappingsMatrix);
            eventSource.KeyPressed += EventSourceOnKeyPressed;
            eventSource.KeyReleased += EventSourceOnKeyReleased;
        }

        private void EventSourceOnKeyReleased(object sender, KeyEventArgs eventArgs)
        {
            SetKeyAsPressed(eventArgs.Value, false);
        }
        
        private void EventSourceOnKeyPressed(object sender, KeyEventArgs eventArgs)
        {
            SetKeyAsPressed(eventArgs.Value, true);
        }

        private void SetKeyAsPressed(Keys key, bool pressed)
        {
            if (keyMappingsByKeyId.ContainsKey(key))
                keyMappingsByKeyId[key].IsPressed = pressed;
        }

        private void InitializeKeyMappings(string keyMappingsMatrix)
        {
            keyMappingsByKeyId = new Dictionary<Keys, KeyMapping>();
            var keyMappings = new List<KeyMapping>();
            var rows = keyMappingsMatrix.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            for(var rowIndex = 0; rowIndex < 11; rowIndex++)
            {
                var rowItems = rows[rowIndex].Split(new[] {"\t", " "}, StringSplitOptions.RemoveEmptyEntries);
                for(var columnIndex = 0; columnIndex <= 7; columnIndex++)
                {
                    var keyNames = rowItems[7 - columnIndex].Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var keyName in keyNames)
                    {
                        var undefinedKey = keyName == ".";

                        if (!undefinedKey && !Enum.IsDefined(typeof(Keys), keyName))
                            throw new InvalidOperationException("Unknown key name: " + keyName);

                        var key = undefinedKey ? 0 : (Keys) Enum.Parse(typeof(Keys), keyName);
                        var mapping = new KeyMapping(key, rowIndex, columnIndex);
                        keyMappings.Add(mapping);
                        keyMappingsByKeyId[key] = mapping;
                    }
                }
            }
        }

        public void WriteToKeyboardMatrixRowSelectionRegister(byte value)
        {
            currentMatrixRow = value & 0x0F;
        }

        public byte ReadFromKeyboardMatrixRowInputRegister()
        {
            byte value = 0xFF;
            var pressedKeysInRow = keyMappingsByKeyId.Values.Where(m => m.Row == currentMatrixRow && m.IsPressed);

            foreach(var mapping in pressedKeysInRow) {
                value = value.WithBit(mapping.Column, 0);
            }

            return value;
        }

        class KeyMapping
        {
            public KeyMapping(Keys key, int row, int column)
            {
                Key = key;
                Row = row;
                Column = column;
            }

            public Keys Key { get; private set; }

            public int Row { get; private set; }

            public int Column { get; private set; }

            public bool IsPressed { get; set; }
        }
    }
}

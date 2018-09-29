using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Xml.Serialization;
using StardewValley;

namespace EquivalentExchange
{
    //unabashedly stolen in its entirety from Cooking Skill. Full of useful utility methods I'll need since I'm cannibalizing so much from Cooking - thank you spacechase0
    class Util
    {
        // http://stackoverflow.com/a/17546909
        public static bool stringDialog(string title, ref string input)
        {
            System.Drawing.Size size = new System.Drawing.Size(200, 70);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = title;

            System.Windows.Forms.TextBox textBox = new TextBox();
            textBox.Size = new System.Drawing.Size(size.Width - 10, 23);
            textBox.Location = new System.Drawing.Point(5, 5);
            textBox.Text = input;
            inputBox.Controls.Add(textBox);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 39);
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new System.Drawing.Point(size.Width - 80, 39);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return (result == DialogResult.OK);
        }

        public static bool yesNoDialog(string title, string text)
        {
            return (MessageBox.Show(title, text, MessageBoxButtons.YesNo) == DialogResult.Yes);
        }

        // http://stackoverflow.com/a/22456034
        public static string serialize<T>(T obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(stream, obj);

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public static T deserialize<T>(string str)
        {
            int beg = str.IndexOf('<');
            string root = str.Substring(beg + 1, str.IndexOf(" xmlns") - beg - 1);
            XmlSerializer serializer = new XmlSerializer(typeof(T)/*, new XmlRootAttribute( root )*/);

            using (TextReader reader = new StringReader(str))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        // http://stackoverflow.com/questions/1879395/how-to-generate-a-stream-from-a-string
        public static Stream stringStream(string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        // http://stackoverflow.com/questions/3303126/how-to-get-the-value-of-private-field-in-c
        public static object GetInstanceField(Type type, object instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }

        public static void SetInstanceField(Type type, object instance, string fieldName, object value)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            field.SetValue(instance, value);
        }

        public static object GetStaticField(Type type, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            return field.GetValue(null);
        }

        public static void SetStaticField(Type type, string fieldName, object value)
        {
            BindingFlags bindFlags = BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            field.SetValue(null, value);
        }

        public static void CallStaticMethod(Type type, string name, object[] args)
        {
            // TODO: Support method overloading
            BindingFlags bindFlags = BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static;
            MethodInfo func = type.GetMethod(name, bindFlags);
            func.Invoke(null, args);
        }

        public static void CallInstanceMethod(Type type, object instance, string name, object[] args)
        {
            // TODO: Support method overloading
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static;
            MethodInfo func = type.GetMethod(name, bindFlags);
            func.Invoke(instance, args);
        }

        public static void DecompileComment(string str)
        {
        }

        // Stolen from SMAPI, same as spacechase0
        public static void InvokeEvent(string name, IEnumerable<Delegate> handlers, object sender)
        {
            var args = new EventArgs();
            foreach (EventHandler handler in handlers.Cast<EventHandler>())
            {
                try
                {
                    handler.Invoke(sender, args);
                }
                catch (Exception e)
                {
                    Log.error($"Exception while handling event {name}:\n{e}");
                }
            }
        }

        public static void InvokeEvent<T>(string name, IEnumerable<Delegate> handlers, object sender, T args)
        {
            foreach (EventHandler<T> handler in handlers.Cast<EventHandler<T>>())
            {
                try
                {
                    handler.Invoke(sender, args);
                }
                catch (Exception e)
                {
                    Log.error($"Exception while handling event {name}:\n{e}");
                }
            }
        }

        public static int GreatestCommonFactor(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public static int LowestCommonDenominator(int a, int b)
        {
            return (a / GreatestCommonFactor(a, b)) * b;
        }

        public static int GetItemValue(int itemId)
        {            
            var o = new StardewValley.Object(itemId, 1);
            return o.sellToStorePrice();
        }

        public static string GetItemName(int itemId)
        {
            var o = new StardewValley.Object(itemId, 1);
            return o.name;
        }

        public static void GiveItemToPlayer(Item item, Farmer player) {
            if (player.couldInventoryAcceptThisItem(item))
            {
                player.addItemToInventory(item);
            }
            else
            {
                Game1.createItemDebris(item, player.getStandingPosition(), player.FacingDirection, player.currentLocation);
            }
        }
        
        public static void TakeItemFromPlayer(int itemId, int quantity, Farmer player)
        {
            while (quantity > 0)
            {
                player.removeFirstOfThisItemFromInventory(itemId);
                --quantity;
            }
        }

        // function used for two things: returns the number of transmutes the player can get off of one slime
        // also the radius at which tool transmutes work. Designed to max out at 4.
        public static int GetAlchemyPowerLevel(int playerAlchemyLevel)
        {
            var modifiedLevel = Math.Max(1, playerAlchemyLevel * 1.6F);
            return (int)Math.Floor(Math.Sqrt(modifiedLevel));
        }
    }
}


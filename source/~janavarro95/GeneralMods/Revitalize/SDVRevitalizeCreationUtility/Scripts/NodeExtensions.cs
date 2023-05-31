/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdvRevitalizeCreationUtility.Scripts
{
    using Godot;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public static class NodeExtensions
    {

        public static void setOwnerRecursive(this Node owner)
        {
            setOwnerRecursive(owner, owner);
        }

        public static void setOwnerRecursive(this Node owner, Node root)
        {
            foreach (Node child in root.GetChildren())
            {
                child.Owner = owner;
                setOwnerRecursive(owner, child);
            }
        }

        /// <summary>
        /// Safely adds a child to the owner node. This is necessary when adding nodes to the active scene tree from outside the main thread.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="child"></param>
        public static void AddChildSafe(this Node owner, Node child, bool legibleUniqueName = false)
        {
            owner.CallDeferred("add_child", child, legibleUniqueName);
        }

        /// <summary>
        /// Gets the child down the given path of nodes indexed by name.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="PathToChild"></param>
        /// <returns></returns>
        public static Node GetChild(this Node root, params string[] PathToChild)
        {
            int currentIndex = 0;
            Node currentNode = root;
            if (currentIndex > PathToChild.Length) return null;
            string currentName = PathToChild[currentIndex];
            while (currentNode != null)
            {
                bool found = false;
                foreach (Node child in currentNode.GetChildren())
                {
                    if (child.Name.Equals(currentName))
                    {
                        found = true;
                        currentNode = child;
                        currentIndex++;
                        if (currentIndex > PathToChild.Length) return null;
                        if (currentIndex == PathToChild.Length) return currentNode;
                        currentName = PathToChild[currentIndex];
                        break;
                    }
                }

                if (found == false) return null;
            }
            return null; //Something went wrong somehow.
        }

        /// <summary>
        /// Gets the child at the given path down the children of nodes indexed by name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        /// <param name="PathToChild"></param>
        /// <returns></returns>
        public static T GetChild<T>(this Node root, params string[] PathToChild) where T : Node
        {
            Node node = GetChild(root, PathToChild);
            if (node == null) return null;
            return (T)node;
        }
    }
}

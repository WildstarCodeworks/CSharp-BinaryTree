using System.Collections;
using System.Collections.Generic;

//Author: WildstarCodeworks
//Created: 11/15/2021
//Last Modified: 11/17/2021

namespace BinaryTree
{
    public class Node<TPayload>
    {
        public int key;
        public List<TPayload> payload;
        public int count;
        public Node<TPayload> parent;
        public Node<TPayload> leftChild;
        public Node<TPayload> rightChild;

        public Node(int key, TPayload payload)
        {
            this.key = key;
            this.payload = new List<TPayload>() { payload };
            this.count = 1;
            parent = null;
            this.leftChild = null;
            this.rightChild = null;
        }
    }//end class

    public class Tree<TPayload>
    {
        public int size; //all nodes presents, count of 2 means 2 nodes.
        public int uniqueSize; //all unique nodes, so a count of 2 still means 1 unique node.
        public string mode; //determines how a 2 children Node<TPayload>is treated when deleted.
        private List<int> keyList;
        private Node<TPayload> root;
        private Node<TPayload> searchResult;

        public Tree()
        {
            root = null;
            mode = "predecessor";
            size = 0;
            uniqueSize = 0;
            keyList = new List<int>();
        }

        public void SwitchModes()
        {
            if (mode == "predecessor") mode = "successor";
            else if (mode == "successor") mode = "predecessor";
            else mode = "predecessor"; //somehow invalid, force it to valid
        }

        public void DisplayTree()
        {
            //To display the tree to a console for debug and educational purposes
            System.Console.Write(DisplayTree_MakeString());
        }

        public string DisplayTree_MakeString()
        {
            string displayString = "=============================\n";
            displayString += "Values read from left-most node to the right-most node\n";
            displayString += string.Format("Tree Size = {0}, Tree Unique Size = {1}\n", size, uniqueSize);
            displayString += DisplayTree_MakeStringRecursive(root);
            displayString += "=============================\n";
            return displayString;
        }

        private string DisplayTree_MakeStringRecursive(Node<TPayload> currentNode)
        {
            if (currentNode == null) return "";
            string returnString = "";

            returnString += DisplayTree_MakeStringRecursive(currentNode.leftChild);
            returnString += DisplayTree_MakeStringNode(currentNode);
            returnString += DisplayTree_MakeStringRecursive(currentNode.rightChild);

            return returnString;
        }

        private string DisplayTree_MakeStringNode(Node<TPayload> node)
        {
            string output = string.Format("{0} :: (Count = {1}, payload = [{2}])", node.key, node.count, string.Join(",", node.payload));
            if (node.leftChild != null) output += string.Format(" [Left = {0}]", node.leftChild.key);
            if (node.rightChild != null) output += string.Format(" [Right = {0}]", node.rightChild.key);
            if (node.parent != null) output += string.Format(" [Parent = {0}]", node.parent.key);
            else output += "<ROOT>";
            output += "\n";
            return output;
        }

        public void Insert(int value, TPayload payload)
        {
            size += 1;
            if (root == null)
            {
                root = new Node<TPayload>(value, payload);
                uniqueSize += 1;
            }
            else InsertRecursive(root, new Node<TPayload>(value, payload));
        }

        private void InsertRecursive(Node<TPayload> currentNode, Node<TPayload> newNode)
        {
            //search the left
            if (newNode.key < currentNode.key)
            {
                if (currentNode.leftChild == null)
                {
                    currentNode.leftChild = newNode;
                    newNode.parent = currentNode;
                    keyList.Add(newNode.key);
                    uniqueSize += 1;
                }
                else InsertRecursive(currentNode.leftChild, newNode);
            } //search the right
            else if (newNode.key > currentNode.key)
            {
                if (currentNode.rightChild == null)
                {
                    currentNode.rightChild = newNode;
                    newNode.parent = currentNode;
                    keyList.Add(newNode.key);
                    uniqueSize += 1;
                }
                else InsertRecursive(currentNode.rightChild, newNode);
            }
            else //We that node in our tree
            {
                currentNode.count += 1;
                currentNode.payload.Add(newNode.payload[0]); //should only have 1 element
            }

        }

        public void Delete(int deleteKey)
        {
            Node<TPayload> deleteNode = GetNode(deleteKey);

            if (deleteNode != null)
            {
                size -= 1;

                if (deleteNode.count > 1)
                {
                    deleteNode.count -= 1;
                    deleteNode.payload.RemoveAt(deleteNode.count - 1); //It's much faster this way. Don't do 0.
                }
                else
                {
                    if (deleteNode.parent != null) DeleteRecursive(deleteNode.parent, deleteKey); //Yes, the parent. Needed for Recursive logic.
                    else DeleteRecursive(root, deleteKey);
                    keyList.Remove(deleteKey);
                    uniqueSize -= 1;
                }
            }
        }

        private Node<TPayload> DeleteRecursive(Node<TPayload> node, int key)
        {
            if (node == null) return node;

            if (key < node.key) node.leftChild = DeleteRecursive(node.leftChild, key);
            else if (key > node.key) node.rightChild = DeleteRecursive(node.rightChild, key);
            else //key == node.key
            {
                if (node.leftChild == null && node.rightChild == null) // 0 children
                {
                    if (node == root) root = null;
                    node = null;
                }
                else if (node.leftChild != null && node.rightChild != null) //2 children
                {
                    if (mode == "predecessor") GetMaxNodeRecursive(node.leftChild);
                    else GetMinNodeRecursive(node.rightChild); //mode == "successor"

                    node.key = searchResult.key;
                    node.payload = searchResult.payload;
                    node.count = searchResult.count;

                    if (mode == "predecessor") node.leftChild = DeleteRecursive(node.leftChild, searchResult.key);
                    else node.rightChild = DeleteRecursive(node.rightChild, searchResult.key); //mode == "successor"
                }
                else //1 child
                {
                    Node<TPayload> child = (node.leftChild != null) ? node.leftChild : node.rightChild;
                    if (node == root) root = child;
                    node = child;
                    node.parent = node.parent.parent; //get the grandparent
                }
            }
            return node;
        }

        public bool ContainsNode(int keyToCheck)
        {
            if (GetNode(keyToCheck) != null) return true;
            else return false;
        }

        public bool ContainsPayload(int keyToCheck, TPayload elementToCheck)
        {
            Node<TPayload> node = GetNode(keyToCheck);

            if (node != null)
            {
                for (int n = 0; n < node.count; n++) //If you count the payload using List.Count it slows it down a lot.
                    if (node.payload[n].Equals(elementToCheck))
                        return true;
            }
            //We never found it, so...
            return false;
        }

        public List<int> GetKeyList()
        {
            return keyList;
        }

        public TPayload GetPayload(int key)
        {
            GetNode(key);
            if (searchResult != null) return searchResult.payload[0];
            else return default(TPayload);
        }

        public Node<TPayload> GetNode(int key)
        {
            searchResult = null;
            GetNodeRecursive(root, key);
            return searchResult;
        }

        public Node<TPayload> GetMaxNode()
        {
            GetMaxNodeRecursive(root);
            return searchResult;
        }

        public Node<TPayload> GetMinNode()
        {
            GetMinNodeRecursive(root);
            return searchResult;
        }
        private void GetNodeRecursive(Node<TPayload> node, int keyToFind)
        {
            if (node == null) return;

            if (keyToFind == node.key) searchResult = node;
            else if (keyToFind < node.key) GetNodeRecursive(node.leftChild, keyToFind);
            else GetNodeRecursive(node.rightChild, keyToFind);
        }

        private void GetMaxNodeRecursive(Node<TPayload> node)
        {
            if (node.rightChild != null) GetMaxNodeRecursive(node.rightChild);
            else searchResult = node;
        }

        private void GetMinNodeRecursive(Node<TPayload> node)
        {
            if (node.leftChild != null) GetMinNodeRecursive(node.leftChild);
            else searchResult = node;
        }
    }//end class
}//end namespace

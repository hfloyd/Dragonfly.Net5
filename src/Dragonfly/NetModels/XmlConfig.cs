namespace Dragonfly.NetModels
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using Dragonfly.NetHelpers;

    /// <summary>
    /// The class which represents a configuration xml file
    /// </summary>
    public class XmlConfig : IDisposable
    {
        private const string ThisClassName = "Dragonfly.NetHelpers.XmlConfig";

        //Originally from http://www.codeproject.com/Articles/16953/XML-configuration-files-made-simple-at-last

        private XmlDocument xmldoc;
        private string originalFile;
        private bool commitonunload = true;
        private bool cleanuponsave = false;


        /// <summary>
        /// Create an XmlConfig from an empty xml file 
        /// containing only the rootelement named as 'xml'
        /// </summary>
        public XmlConfig()
        {
            this.xmldoc = new XmlDocument();
            this.LoadXmlFromString("<xml></xml>");
        }

        /// <summary>
        /// Create an XmlConfig from an existing file, or create a new one
        /// </summary>
        /// <param name="loadfromfile">
        /// Path and filename from where to load the xml file
        /// </param>
        /// <param name="create">
        /// If file does not exist, create it, or throw an exception?
        /// </param>
        public XmlConfig(string loadfromfile, bool create)
        {
            string MappedConfigPath = Files.GetMappedPath(loadfromfile);

            this.xmldoc = new XmlDocument();
            this.LoadXmlFromFile(MappedConfigPath, create);
        }

        /// <summary>
        /// Check XML file if it conforms the config xml restrictions
        /// 1. No nodes with two children of the same name
        /// 2. Only alphanumerical names
        /// </summary>
        /// <param name="silent">
        /// Whether to return a true/false value, or throw an exception on failiure
        /// </param>
        /// <returns>
        /// True on success and in case of silent mode false on failiure
        /// </returns>
        public bool ValidateXML(bool silent)
        {
            if (!this.Settings.Validate())
            {
                if (silent)
                    return false;
                else
                    throw new Exception("This is not a valid configuration xml file! Probably duplicate children with the same names, or non-alphanumerical tagnames!");
            }
            else
                return true;
        }

        /// <summary>
        /// Strip empty nodes from the whole tree.
        /// </summary>
        public void Clean()
        {
            this.Settings.Clean();
        }

        /// <summary>
        /// Whether to clean the tree by stripping out
        /// empty nodes before saving the XML config
        /// file
        /// </summary>
        public bool CleanUpOnSave
        {
            get { return this.cleanuponsave; }
            set { this.cleanuponsave = value; }
        }

        /// <summary>
        /// When unloading the current XML config file
        /// shold any changes be saved back to the file?
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>Only applies if it was loaded from a local file</item>
        /// <item>True by default</item>
        /// </list>
        /// </remarks>
        public bool CommitOnUnload
        {
            get { return this.commitonunload; }
            set { this.commitonunload = value; }
        }

        /// <summary>
        /// Save any modifications to the XML file before destruction
        /// if CommitOnUnload is true
        /// </summary>
        public void Dispose()
        {
            if (this.CommitOnUnload) this.Commit();
        }

        /// <summary>
        /// Load a new XmlDocument from a file
        /// </summary>
        /// <param name="filename">
        /// Path and filename from where to load the xml file
        /// </param>
        /// <param name="create">
        /// If file does not exist, create it, or throw an exception?
        /// </param>
        public void LoadXmlFromFile(string filename, bool create)
        {
            if (this.CommitOnUnload) this.Commit();
            try
            {
                this.xmldoc.Load(filename);
            }
            catch
            {
                if (!create)
                    throw new Exception("xmldoc.Load() failed! Probably file does NOT exist!");
                else
                {
                    this.xmldoc.LoadXml("<xml></xml>");
                    this.Save(filename);
                }
            }
            this.ValidateXML(false);
            this.originalFile = filename;

        }

        /// <summary>
        /// Load a new XmlDocument from a file
        /// </summary>
        /// <param name="filename">
        /// Path and filename from where to load the xml file
        /// </param>
        /// <remarks>
        /// Throws an exception if file does not exist
        /// </remarks>
        public void LoadXmlFromFile(string filename)
        {
            this.LoadXmlFromFile(filename, false);
        }

        /// <summary>
        /// Load a new XmlDocument from a string
        /// </summary>
        /// <param name="xml">
        /// XML string
        /// </param>
        public void LoadXmlFromString(string xml)
        {
            if (this.CommitOnUnload) this.Commit();
            this.xmldoc.LoadXml(xml);
            this.originalFile = null;
            this.ValidateXML(false);
        }

        /// <summary>
        /// Load an empty XmlDocument
        /// </summary>
        /// <param name="rootelement">
        /// Name of root element
        /// </param>
        public void NewXml(string rootelement)
        {
            if (this.CommitOnUnload) this.Commit();
            this.LoadXmlFromString(string.Format("<{0}></{0}>", rootelement));
        }

        /// <summary>
        /// Save configuration to an xml file
        /// </summary>
        /// <param name="filename">
        /// Path and filname where to save
        /// </param>
        public void Save(string filename)
        {
            string MappedFilenamePath = Files.GetMappedPath(filename);

            this.ValidateXML(false);
            if (this.CleanUpOnSave)
                this.Clean();
            this.xmldoc.Save(MappedFilenamePath);
            this.originalFile = MappedFilenamePath;
        }

        /// <summary>
        /// Save configuration to a stream
        /// </summary>
        /// <param name="stream">
        /// Stream where to save
        /// </param>
        public void Save(System.IO.Stream stream)
        {
            this.ValidateXML(false);
            if (this.CleanUpOnSave)
                this.Clean();
            this.xmldoc.Save(stream);
        }

        /// <summary>
        /// If loaded from a file, commit any changes, by overwriting the file
        /// </summary>
        /// <returns>
        /// True on success
        /// False on failiure, probably due to the file was not loaded from a file
        /// </returns>

        public bool Commit()
        {
            if (this.originalFile != null) { this.Save(this.originalFile); return true; } else { return false; }
        }

        /// <summary>
        /// If loaded from a file, trash any changes, and reload the file
        /// </summary>
        /// <returns>
        /// True on success
        /// False on failiure, probably due to file was not loaded from a file
        /// </returns>
        public bool Reload()
        {
            if (this.originalFile != null) { this.LoadXmlFromFile(this.originalFile); return true; } else { return false; }
        }

        /// <summary>
        /// Gets the root ConfigSetting
        /// </summary>
        public ConfigSetting Settings
        {
            get { return new ConfigSetting(this.xmldoc.DocumentElement); }
        }

    }

    /// <summary>
    /// Represents a Configuration Node in the XML file
    /// </summary>
    public class ConfigSetting
    {
        //Originally from http://www.codeproject.com/Articles/16953/XML-configuration-files-made-simple-at-last

        /// <summary>
        /// The node from the XMLDocument, which it describes
        /// </summary>
        private XmlNode node;

        /// <summary>
        /// This class cannot be constructed directly. You will need to give a node to describe
        /// </summary>
        private ConfigSetting()
        {
            throw new Exception("Cannot be created directly. Needs a node parameter");
        }

        /// <summary>
        /// Creates an instance of the class
        /// </summary>
        /// <param name="node">
        /// the XmlNode to describe
        /// </param>
        public ConfigSetting(XmlNode node)
        {
            if (node == null)
                throw new Exception("Node parameter can NOT be null!");
            this.node = node;
        }

        /// <summary>
        /// The Name of the element it describes
        /// </summary>
        /// <remarks>Read only property</remarks>        
        public string Name
        {
            get
            {
                return this.node.Name;
            }
        }

        /// <summary>
        /// Gets the number of children of the specific node
        /// </summary>
        /// <param name="unique">
        /// If true, get only the number of children with distinct names.
        /// So if it has two nodes with "foo" name, and three nodes
        /// named "bar", the return value will be 2. In the same case, if unique
        /// was false, the return value would have been 2 + 3 = 5
        /// </param>
        /// <returns>
        /// The number of (uniquely named) children
        /// </returns>
        public int ChildCount(bool unique)
        {
            //warning Code in ChildCoutn(bool) is NOT optimised. If you can help me out with a better algorithm selecting unique child names (probably using XPath), please contact me at axos88@gmail.com Thanks!
            IList<string> names = this.ChildrenNames(unique);
            if (names != null)
                return names.Count;
            else
                return 0;
        }

        /// <summary>
        /// Gets the names of children of the specific node
        /// </summary>
        /// <param name="unique">
        /// If true, get only distinct names.
        /// So if it has two nodes with "foo" name, and three nodes
        /// named "bar", the return value will be {"bar","foo"} . 
        /// In the same case, if unique was false, the return value 
        /// would have been {"bar","bar","bar","foo","foo"}
        /// </param>
        /// <returns>
        /// An IList object with the names of (uniquely named) children
        /// </returns>
        public IList<String> ChildrenNames(bool unique)
        {
#warning Code in ChildrenNames(bool) is NOT optimised. If you can help me out with a better algorithm selecting unique child names (probably using XPath), please contact me at axos88@gmail.com Thanks!

            if (this.node.ChildNodes.Count == 0)
                return null;
            List<String> stringlist = new List<string>();

            foreach (XmlNode achild in this.node.ChildNodes)
            {
                string name = achild.Name;
                if ((!unique) || (!stringlist.Contains(name)))
                    stringlist.Add(name);
            }

            stringlist.Sort();
            return stringlist;
        }

        /// <summary>
        /// An IList compatible object describing each and every child node
        /// </summary>
        /// <remarks>Read only property</remarks>
        public IList<ConfigSetting> Children()
        {
            if (this.ChildCount(false) == 0)
                return null;
            List<ConfigSetting> list = new List<ConfigSetting>();

            foreach (XmlNode achild in this.node.ChildNodes)
            {
                list.Add(new ConfigSetting(achild));
            }
            return list;
        }
        /// <summary>
        /// Get all children with the same name, specified in the name parameter
        /// </summary>
        /// <param name="name">
        /// An alphanumerical string, containing the name of the child nodes to return
        /// </param>
        /// <returns>
        /// An array with the child nodes with the specified name, or null 
        /// if no childs with the specified name exist
        /// </returns>
        public IList<ConfigSetting> GetNamedChildren(String name)
        {
            foreach (Char c in name)
                if (!Char.IsLetterOrDigit(c))
                    throw new Exception("Name MUST be alphanumerical!");
            XmlNodeList xmlnl = this.node.SelectNodes(name);
            int NodeCount = xmlnl.Count;
            List<ConfigSetting> css = new List<ConfigSetting>();
            foreach (XmlNode achild in xmlnl)
            {
                css.Add(new ConfigSetting(achild));
            }
            return css;
        }

        /// <summary>
        /// Gets the number of childs with the specified name
        /// </summary>
        /// <param name="name">
        /// An alphanumerical string with the name of the nodes to look for
        /// </param>
        /// <returns>
        /// An integer with the count of the nodes
        /// </returns>
        public int GetNamedChildrenCount(String name)
        {
            foreach (Char c in name)
                if (!Char.IsLetterOrDigit(c))
                    throw new Exception("Name MUST be alphanumerical!");
            return this.node.SelectNodes(name).Count;
        }

        /// <summary>
        /// String value of the specific Configuration Node
        /// </summary>
        public string Value
        {
            get
            {
                XmlNode xmlattrib = this.node.Attributes.GetNamedItem("value");
                if (xmlattrib != null) return xmlattrib.Value; else return "";
            }

            set
            {
                XmlNode xmlattrib = this.node.Attributes.GetNamedItem("value");
                if (value != "")
                {
                    if (xmlattrib == null) xmlattrib = this.node.Attributes.Append(this.node.OwnerDocument.CreateAttribute("value"));
                    xmlattrib.Value = value;
                }
                else if (xmlattrib != null) this.node.Attributes.RemoveNamedItem("value");
            }
        }

        /// <summary>
        /// int value of the specific Configuration Node
        /// </summary>
        public int intValue
        {
            get { int i; int.TryParse(this.Value, out i); return i; }
            set { this.Value = value.ToString(); }

        }
        /// <summary>
        /// bool value of the specific Configuration Node
        /// </summary>
        public bool boolValue
        {
            get { bool b; bool.TryParse(this.Value, out b); return b; }
            set { this.Value = value.ToString(); }
        }

        /// <summary>
        /// float value of the specific Configuration Node
        /// </summary>
        public float floatValue
        {
            get { float f; float.TryParse(this.Value, out f); return f; }
            set { this.Value = value.ToString(); }

        }

        /// <summary>
        /// Get a specific child node
        /// </summary>
        /// <param name="path">
        /// The path to the specific node. Can be either only a name, or a full path separated by '/' or '\'
        /// </param>
        /// <example>
        /// <code>
        /// XmlConfig conf = new XmlConfig("configuration.xml");
        /// screenname = conf.Settings["screen"].Value;
        /// height = conf.Settings["screen/height"].IntValue;
        ///  // OR
        /// height = conf.Settings["screen"]["height"].IntValue;
        /// </code>
        /// </example>
        /// <returns>
        /// The specific child node
        /// </returns>
        public ConfigSetting this[string path]
        {
            get
            {
                char[] separators = { '/', '\\' };
                path.Trim(separators);
                String[] pathsection = path.Split(separators);

                XmlNode selectednode = this.node;
                XmlNode newnode;

                foreach (string asection in pathsection)
                {
                    String nodename, nodeposstr;
                    int nodeposition;
                    int indexofdiez = asection.IndexOf('#');

                    if (indexofdiez == -1) // No position defined, take the first one by default
                    {
                        nodename = asection;
                        nodeposition = 1;
                    }
                    else
                    {
                        nodename = asection.Substring(0, indexofdiez); // Node name is before the diez character
                        nodeposstr = asection.Substring(indexofdiez + 1);
                        if (nodeposstr == "#") // Double diez means he wants to create a new node
                        {
                            ConfigSetting csTemp = new ConfigSetting(selectednode);
                            nodeposition = csTemp.GetNamedChildrenCount(nodename) + 1;
                        }
                        else
                            nodeposition = int.Parse(nodeposstr);
                    }

                    // Verify name
                    foreach (char c in nodename)
                    { if ((!Char.IsLetterOrDigit(c))) return null; }

                    String transformedpath = string.Format("{0}[{1}]", nodename, nodeposition);
                    newnode = selectednode.SelectSingleNode(transformedpath);

                    while (newnode == null)
                    {
                        XmlElement newelement = selectednode.OwnerDocument.CreateElement(nodename);
                        selectednode.AppendChild(newelement);
                        newnode = selectednode.SelectSingleNode(transformedpath);
                    }
                    selectednode = newnode;
                }

                return new ConfigSetting(selectednode);
            }
        }

        /// <summary>
        /// Check if the node conforms with the config xml restrictions
        /// 1. No nodes with two children of the same name
        /// 2. Only alphanumerical names
        /// </summary>
        /// <returns>
        /// True on success and false on failiure
        /// </returns>        
        public bool Validate()
        {
            // Check this node's name for validity
            foreach (Char c in this.Name)
                if (!Char.IsLetterOrDigit(c))
                    return false;

            // If there are no children, the node is valid.
            // If there the node has other children, check all of them for validity
            if (this.ChildCount(false) == 0)
                return true;
            else
            {
                foreach (ConfigSetting cs in this.Children())
                {
                    if (!cs.Validate())
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Removes any empty nodes from the tree, 
        /// that is it removes a node, if it hasn't got any
        /// children, or neither of its children have got a value.
        /// </summary>
        public void Clean()
        {
            if (this.ChildCount(false) != 0)
                foreach (ConfigSetting cs in this.Children())
                {
                    cs.Clean();
                }
            if ((this.ChildCount(false) == 0) && (this.Value == ""))
                this.Remove();
        }

        /// <summary>
        /// Remove the specific node from the tree
        /// </summary>
        public void Remove()
        {
            if (this.node.ParentNode == null) return;
            this.node.ParentNode.RemoveChild(this.node);
        }

        /// <summary>
        /// Remove all children of the node, but keep the node itself
        /// </summary>
        public void RemoveChildren()
        {
            this.node.RemoveAll();
        }


    }
}
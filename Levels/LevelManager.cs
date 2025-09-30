using DinaFramework.Functions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace DinaFramework.Levels
{
    /// <summary>
    /// Gère le chargement et la construction des niveaux Tiled (.tmx).
    /// </summary>
    public class LevelManager
    {
        ContentManager _content;
        XDocument? _doc;
        XElement? _root;

        /// <summary>
        /// Initialise un gestionnaire de niveaux avec un ContentManager.
        /// </summary>
        /// <param name="content">ContentManager pour charger les ressources (textures, etc.).</param>
        public LevelManager(ContentManager content)
        {
            _content = content;
        }
        /// <summary>
        /// Charge un fichier TMX (Tiled) et construit un objet Level.
        /// </summary>
        /// <param name="tmxPath">Chemin du fichier .tmx.</param>
        /// <param name="embedded">Indique si la ressource est embarquée dans l’assembly.</param>
        /// <param name="assembly">Nom de l'assembly</param>
        /// <returns>Un objet Level prêt à être utilisé.</returns>
        public Level Load(string tmxPath, bool embedded = false, Assembly? assembly = null)
        {
            if (embedded)
            {
                ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));
                using Stream? stream = assembly.GetManifestResourceStream(tmxPath);
                if (stream == null)
                    throw new FileNotFoundException($"{tmxPath} introuvable dans les ressources embarquées.");
                _doc = XDocument.Load(stream);
            }
            else
            {
                using Stream? stream = TitleContainer.OpenStream(tmxPath);
                if (stream == null)
                    throw new FileNotFoundException($"{tmxPath} introuvable.");
                _doc = XDocument.Load(stream);
            }

            _root = _doc.Root ?? throw new InvalidOperationException("TMX file invalid");

            Level level = new Level()
            {
                Orientation = Enum.TryParse(_root.Attribute("orientation")?.Value, out TiledOrientation orientation) ? orientation : TiledOrientation.Orthogonal,
                RenderOrder = _root.Attribute("renderorder")?.Value ?? "left-up",
                Width = int.Parse(_root.Attribute("width")?.Value ?? "0", CultureInfo.InvariantCulture),
                Height = int.Parse(_root.Attribute("height")?.Value ?? "0", CultureInfo.InvariantCulture),
                TileWidth = int.Parse(_root.Attribute("tilewidth")?.Value ?? "0", CultureInfo.InvariantCulture),
                TileHeight = int.Parse(_root.Attribute("tileheight")?.Value ?? "0", CultureInfo.InvariantCulture),
                Infinite = int.Parse(_root.Attribute("infinite")?.Value ?? "0", CultureInfo.InvariantCulture),
            };

            foreach (var element in _root.Elements())
            {
                switch (element.Name.ToString())
                {
                    case "tileset":
                        TiledTileset tileset = LoadTileset(element);
                        level.AddTileset(tileset);
                        break;
                    case "group":
                        TiledGroup group = LoadGroup(element);
                        level.AddGroup(group);
                        break;
                    case "layer":
                        TiledLayer layer = LoadLayer(element);
                        level.AddLayer(layer);
                        break;
                    case "objectgroup":
                        TiledObjectGroup layerobjectgroup = LoadObjectGroup(element);
                        level.AddObjectGroup(layerobjectgroup);
                        break;
                    case "imagelayer":
                        TiledImageLayer imagelayer = LoadImageLayer(element);
                        level.AddImageLayer(imagelayer);
                        break;
                }
            }
            return level;
        }
        private TiledTileset LoadTileset(XElement element)
        {
            var source = GetChildAttribute(element, "image", "source", string.Empty);
            var contentfile = source.Replace(Path.GetExtension(source), string.Empty, StringComparison.CurrentCultureIgnoreCase);
            TiledTileset tileset = new TiledTileset()
            {
                FirstGid = GetAttribute(element, "firstgid", 1),
                Name = GetAttribute(element, "name", string.Empty),
                TileWidth = GetAttribute(element, "tilewidth", 0),
                TileHeight = GetAttribute(element, "tileheight", 0),
                Spacing = GetAttribute(element, "spacing", 0),
                Margin = GetAttribute(element, "margin", 0),
                TileCount = GetAttribute(element, "tilecount", 0),
                Columns = GetAttribute(element, "columns", 0),
                Visible = GetAttribute(element, "visible", false),
                TileOffset = new Vector2(GetChildAttribute(element, "tileoffset", "x", 0f), GetChildAttribute(element, "tileoffset", "y", 0f)),
                HorizontalFlip = GetChildAttribute(element, "transformations", "hflip", false),
                VerticalFlip = GetChildAttribute(element, "transformations", "vflip", false),
                Rotate = GetChildAttribute(element, "transformations", "rotate", false),
                PreferUntransformed = GetChildAttribute(element, "transformations", "preferuntransformed", false),
                Transparency = GetChildAttribute(element, "image", "trans", Color.Transparent),
                Image = _content.Load<Texture2D>(contentfile),
                Properties = GetProperties(element),
            };
            int columns = tileset.Columns > 0 ? tileset.Columns : Math.Max(1, tileset.TileCount);
            for (int index = 0; index < tileset.TileCount; index++)
            {
                var row = index / columns;
                var col = index % columns;
                int x = tileset.Margin + col * (tileset.TileWidth + tileset.Spacing);
                int y = tileset.Margin + row * (tileset.TileHeight + tileset.Spacing);
                var width = tileset.TileWidth;
                var height = tileset.TileHeight;
                Rectangle rect = new Rectangle(x, y, width, height);
                var id = tileset.FirstGid + index;
                tileset.Quads[id] = rect;
            }

            return tileset;
        }
        private static TiledLayer LoadLayer(XElement element)
        {
            int width = GetAttribute(element, "width", 0);
            int height = GetAttribute(element, "height", 0);
            TiledLayer layer = new TiledLayer()
            {
                Name = GetAttribute(element, "name", string.Empty),
                Class = GetAttribute(element, "class", string.Empty),
                Width = width,
                Height = height,
                Opacity = GetAttribute(element, "opacity", 1f),
                Visible = GetAttribute(element, "visible", true),
                Properties = GetProperties(element),
            };
            var dataString = element.Element("data")?.Value;
            if (dataString == null)
                throw new InvalidDataException($"The layer {layer.Name} don't have any data.");

            var arrData = dataString.Split([",", "\n", "\r"], StringSplitOptions.RemoveEmptyEntries);
            if (arrData.Length != width * height)
                throw new InvalidDataException($"Layer {layer.Name} expected {width * height} tiles but got {arrData.Length}.");

            layer.Data = new uint[width * height];

            for (int i = 0; i < arrData.Length; i++)
                layer.Data[i] = Convert.ToUInt32(arrData[i], CultureInfo.InvariantCulture);

            return layer;
        }
        private TiledGroup LoadGroup(XElement element)
        {
            var name = GetAttribute(element, "name", string.Empty);
            TiledGroup newGroup = new TiledGroup()
            {
                Name = name,
                Opacity = GetAttribute(element, "opacity", 1f),
                Properties = GetProperties(element),
                Visible = GetAttribute(element, "visible", true),
            };
            foreach (var child in element.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "group":
                        TiledGroup childGroup = LoadGroup(child);
                        childGroup.Parent = name;
                        newGroup.AddGroup(childGroup);
                        break;
                    case "layer":
                        TiledLayer layer = LoadLayer(child);
                        layer.Parent = name;
                        newGroup.AddLayer(layer);
                        break;
                    case "objectgroup":
                        TiledObjectGroup objectgroup = LoadObjectGroup(child);
                        objectgroup.Parent = name;
                        newGroup.AddObject(objectgroup);
                        break;
                    case "imagelayer":
                        var imagelayer = LoadImageLayer(child);
                        imagelayer.Parent = name;
                        newGroup.AddImage(imagelayer);
                        break;
                }
            }
            return newGroup;
        }
        private static TiledObjectGroup LoadObjectGroup(XElement element)
        {
            var name = GetAttribute(element, "name", string.Empty);

            TiledObjectGroup newObjectGroup = new TiledObjectGroup()
            {
                Name = name,
                Opacity = GetAttribute(element, "opacity", 1f),
                Visible = GetAttribute(element, "visible", true),
            };

            foreach (var child in element.Elements())
            {
                var x = GetAttribute(child, "x", 0);
                var y = GetAttribute(child, "y", 0);
                var width = GetAttribute(child, "width", 0);
                var height = GetAttribute(child, "height", 0);

                TiledObjectType tiledObjectType = TiledObjectType.Default;
                
                var objectType = child.FirstNode;
                if (objectType != null)
                {
                    Enum.TryParse(objectType.ToString(), true, out tiledObjectType);
                }

                TiledObject tiledObject = new TiledObject()
                {
                    ID = GetAttribute(child, "id", 0),
                    Name = GetAttribute(child, "name", string.Empty),
                    Class = GetAttribute(child, "type", string.Empty),
                    GID = GetAttribute(child, "gid", 0u),
                    Bounds = new Rectangle(x, y, width, height),
                    Rotation = GetAttribute(child, "rotation", 0f),
                    Shape = tiledObjectType,
                    Properties = GetProperties(child),
                    Parent = name,
                };
                newObjectGroup.AddObject(tiledObject);
            }

            return newObjectGroup;
        }
        private TiledImageLayer LoadImageLayer(XElement element)
        {
            // TODO: à contrôler

            var x = GetAttribute(element, "offsetx", 0f);
            var y = GetAttribute(element, "offsety", 0f);
            TiledImageLayer tiledimage = new TiledImageLayer()
            {
                Name = GetAttribute(element, "name", string.Empty),
                Class = GetAttribute(element, "class", string.Empty),
                Opacity = GetAttribute(element, "opacity", 1f),
                Offset = new Vector2(x, y),
                RepeatX = GetAttribute(element, "repeatx", false),
                RepeatY = GetAttribute(element, "repeaty", false),
                Texture = _content.Load<Texture2D>(GetChildAttribute(element, "image", "source", string.Empty)),
                Transparency = GetChildAttribute(element, "image", "trans", Color.Transparent),
                Visible = GetAttribute(element, "visible", true),
                Properties = GetProperties(element),
            };
            return tiledimage;
        }

        private static T GetChildAttribute<T>(XElement parent, string childName, string attributeName, T defaultValue)
        {
            var child = parent.Element(childName);
            if (child == null)
                return defaultValue;

            return GetAttribute(child, attributeName, defaultValue);
        }
        private static T GetAttribute<T>(XElement element, string attributeName, T defaultValue)
        {
            var attribute = element.Attribute(attributeName);
            if (attribute == null)
                return defaultValue;

            try
            {
                if (typeof(T) == typeof(string))
                    return (T)(object)attribute.Value;

                if (typeof(T) == typeof(Color))
                    return (T)(object)DinaFunctions.FromHex(attribute.Value);

                if (typeof(T).IsEnum)
                    return (T)Enum.Parse(typeof(T), attribute.Value, ignoreCase: true);

                return (T)Convert.ChangeType(attribute.Value, typeof(T), CultureInfo.CurrentCulture);
            }
            catch (FormatException) { }
            catch (InvalidCastException) { }
            catch (OverflowException) { }

            return defaultValue;
        }
        private static List<IProperty> GetProperties(XElement element)
        {
            List<IProperty> properties = [];

            var child = element.Element("properties");
            if (child != null)
            {
                foreach (var property in child.Elements())
                {
                    var id = GetAttribute(property, "id", 0);
                    var name = GetAttribute(property, "name", string.Empty);
                    var type = GetAttribute(property, "type", TiledPropertyType.String);
                    switch (type)
                    {
                        case TiledPropertyType.Bool:
                        {
                            TiledProperty<bool> tiledproperty = new TiledProperty<bool>()
                            {
                                ID = id,
                                Name = name,
                                Type = type,
                                Value = GetAttribute(property, "value", false)
                            };
                            properties.Add(tiledproperty);
                            break;
                        }
                        case TiledPropertyType.Color:
                        {
                            TiledProperty<Color> tiledproperty = new TiledProperty<Color>()
                            {
                                ID = id,
                                Name = name,
                                Type = type,
                                Value = GetAttribute(property, "value", Color.Transparent)
                            };
                            properties.Add(tiledproperty);
                            break;
                        }
                        case TiledPropertyType.File:
                        {
                            TiledProperty<string> tiledproperty = new TiledProperty<string>()
                            {
                                ID = id,
                                Name = name,
                                Type = type,
                                Value = GetAttribute(property, "value", string.Empty)
                            };
                            properties.Add(tiledproperty);
                            break;
                        }
                        case TiledPropertyType.Float:
                        {
                            TiledProperty<float> tiledproperty = new TiledProperty<float>()
                            {
                                ID = id,
                                Name = name,
                                Type = type,
                                Value = GetAttribute(property, "value", 0f)
                            };
                            properties.Add(tiledproperty);
                            break;
                        }
                        case TiledPropertyType.Int:
                        {
                            TiledProperty<int> tiledproperty = new TiledProperty<int>()
                            {
                                ID = id,
                                Name = name,
                                Type = type,
                                Value = GetAttribute(property, "value", 0)
                            };
                            properties.Add(tiledproperty);
                            break;
                        }
                        case TiledPropertyType.Object:
                        {
                            TiledProperty<int> tiledproperty = new TiledProperty<int>()
                            {
                                ID = id,
                                Name = name,
                                Type = type,
                                Value = GetAttribute(property, "value", 0)
                            };
                            properties.Add(tiledproperty);
                            break;
                        }
                        case TiledPropertyType.String:
                        {
                            TiledProperty<string> tiledproperty = new TiledProperty<string>()
                            {
                                ID = id,
                                Name = name,
                                Type = type,
                                Value = GetAttribute(property, "value", string.Empty)
                            };
                            properties.Add(tiledproperty);
                            break;
                        }
                    }
                }
            }
            return properties;
        }
    }
}

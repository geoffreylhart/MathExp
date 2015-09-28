using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MathExp
{
    public static class BinaryReaderWriterHelper
    {
        public static VertexPositionColor[] ReadVertices(this BinaryReader reader, Color color)
        {
            VertexPositionColor[] response = new VertexPositionColor[reader.ReadInt32()];
            for (int i = 0; i < response.Length; i++)
            {
                response[i] = new VertexPositionColor(new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()), color);
            }
            return response;
        }
        public static short[] ReadShorts(this BinaryReader reader)
        {
            short[] response = new short[reader.ReadInt32()];
            for (int i = 0; i < response.Length; i++)
            {
                response[i] = reader.ReadInt16();
            }
            return response;
        }
        public static void WriteVertices(this BinaryWriter writer, VertexPositionColor[] vertices)
        {
            writer.Write(vertices.Length);
            foreach (var vertex in vertices)
            {
                writer.Write(vertex.Position.X);
                writer.Write(vertex.Position.Y);
                writer.Write(vertex.Position.Z);
            }
        }
        public static void WriteShorts(this BinaryWriter writer, short[] shorts)
        {
            writer.Write(shorts.Length);
            foreach (var index in shorts)
            {
                writer.Write(index);
            }
        }
    }
}

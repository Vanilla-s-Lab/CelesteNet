﻿using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Celeste.Mod.CelesteNet.DataTypes {
    public class DataMoveTo : DataType<DataMoveTo> {

        static DataMoveTo() {
            DataID = "playerMoveTo";
        }

        public bool Force;
        public string SID = "";
        public AreaMode Mode;
        public string Level = "";

        public DataSession? Session;

        public Vector2? Position;

        public override void Read(DataContext ctx, BinaryReader reader) {
            Force = reader.ReadBoolean();
            SID = reader.ReadNullTerminatedString();
            Mode = (AreaMode) reader.ReadByte();
            Level = reader.ReadNullTerminatedString();

            if (reader.ReadBoolean()) {
                Session = new DataSession();
                Session.Read(ctx, reader);
            } else {
                Session = null;
            }

            if (reader.ReadBoolean())
                Position = reader.ReadVector2();
        }

        public override void Write(DataContext ctx, BinaryWriter writer) {
            writer.Write(Force);
            writer.WriteNullTerminatedString(SID);
            writer.Write((byte) Mode);
            writer.WriteNullTerminatedString(Level);

            if (Session == null) {
                writer.Write(false);
            } else {
                writer.Write(true);
                Session.Write(ctx, writer);
            }

            if (Position == null) {
                writer.Write(false);
            } else {
                writer.Write(true);
                writer.Write(Position.Value);
            }
        }

    }
}

using MathExp.Geometry;
using MathExp.PathFinder;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MathExp
{
    class LineEnvironment
    {
        private static float GRAVITY = 0.15f;
        private static float ACCEL = 0.1f;

        private GeometryCollection drawnGeometry = new GeometryCollection();
        private GeometryCollection geometry;
        private NullablePoint selected = new NullablePoint();
        MouseListener mouseListener;
        Particle player = new Particle(new Vector2(400, 200), Vector2.Zero, new Vector2(0, GRAVITY));
        BoundParticle player2 = null;
        Boolean isBound = false;
        Boolean playback = false;
        MathExp.PathFinder.Path recording = null;
        float timeInRecording = 0;
        Particle recordingPointer = null;

        // Load
        public LineEnvironment(string file)
        {
            if (File.Exists(file)) {
                Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None);
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    geometry = new GeometryCollection(reader.ReadVertices(Color.White).ToList(), reader.ReadShorts().ToList());
                }
            } else
            {
                geometry = new GeometryCollection();
            }
            mouseListener = new MouseListener();
            mouseListener.LeftButtonDrag = (start, end) =>
            {
                selected.Position = end;
            };
            mouseListener.RightButtonDrag = (start, end) =>
            {
                var selected2 = geometry.SnapToClosePoint(end);
                drawnGeometry = new GeometryCollection();
                if(selected2 == null)
                {
                    drawnGeometry.Add(selected, new VertexPositionColor(end, Color.Red));
                }else
                {
                    drawnGeometry.Add(selected, selected2);
                }
            };
            mouseListener.RightButtonRelease = (pos) =>
                {
                    drawnGeometry = new GeometryCollection();
                    if (selected.isNull)
                    {
                        geometry.Add(new VertexPositionColor(pos, Color.White));
                    }else
                    {
                        var selected2 = geometry.SnapToClosePoint(pos);
                        if(selected2 == (MathExp.Geometry.Point)selected)
                        {
                            geometry.Remove(selected);
                        }
                        else
                        {
                            geometry.Add(selected, selected2);
                        }
                    }
                };
            mouseListener.Move = (start, end) =>
                {
                    selected.Color = Color.White;
                    selected = geometry.SnapToClosePoint(end);
                    selected.Color = Color.Red;
                };
            mouseListener.LeftButtonRelease = (pos) =>
                {
                    if (selected.isNull)
                    {
                        player.position = new Vector2((float)Math.Round(pos.X/10)*10, (float)Math.Round(pos.Y/10)*10);
                        player.velocity = Vector2.Zero;
                        isBound = false;
                    }
                };
        }

        internal void Save(string file)
        {
            Stream stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None);
            using (var writer = new BinaryWriter(stream))
            {
                writer.WriteVertices(geometry.GetPointsAsArray());
                writer.WriteShorts(geometry.GetLinesAsIndexArray());
            }
        }

        internal void Draw(GraphicsDevice graphicsDevice, BasicEffect basicEffect)
        {
            drawnGeometry.Draw(graphicsDevice, basicEffect);
            geometry.Draw(graphicsDevice, basicEffect);
            if (playback)
            {
                timeInRecording++;
                recordingPointer.position = recording.absolutePosAt(timeInRecording);
                if (timeInRecording > recording.totalTime)
                {
                    playback = false;
                }
                recordingPointer.Draw(graphicsDevice, basicEffect);
            }
            else if(isBound)
            {
                player2.Draw(graphicsDevice, basicEffect);
            }
            else
            {
                player.Draw(graphicsDevice, basicEffect);
            }
        }

        internal void Update()
        {
            mouseListener.Update();
            var keystate = Keyboard.GetState();
            if(keystate.IsKeyDown(Keys.Z) && !playback && isBound) // TODO: we can improve this to work even when unbound
            {
                var target = geometry.SnapToClosePoint(mouseListener.Transform(Mouse.GetState()));
                recording = PathFinding.PathTo(player2, target, geometry, ACCEL, GRAVITY);
                if (recording != null)
                {
                    timeInRecording = 0;
                    recordingPointer = new Particle(recording.absolutePosAt(0), Vector2.Zero, Vector2.Zero);
                    playback = true;
                }
            }
            if (!playback)
            {
                if (!isBound)
                {
                    var collision = geometry.FirstCollision(player);
                    if (collision != null)
                    {
                        isBound = true;
                        player2 = collision;
                    }
                }
                if (isBound)
                {
                    bool upsideDown = (((Vector2)player2.boundTo).X < 0) != player2.onCW;
                    if (upsideDown)
                    {
                        isBound = false;
                        player.position = player2.boundTo.p1 + Vector2.Multiply(player2.boundTo, (float)player2.g);
                        player.velocity = Vector2.Multiply(player2.boundTo, (float)player2.gv) + player.gravity;
                        player.position += player.velocity;
                        player.velocity += player.gravity;
                    }
                    else
                    {
                        player2.g += player2.gv;
                        player2.gv += player2.ga;
                        if (keystate.IsKeyDown(Keys.A))
                        {
                            player2.gv += ACCEL * ((player2.onCW) ? 1 : -1) / ((Vector2)player2.boundTo).Length();
                        }
                        if (keystate.IsKeyDown(Keys.D))
                        {
                            player2.gv -= ACCEL * ((player2.onCW) ? 1 : -1) / ((Vector2)player2.boundTo).Length();
                        }
                        if (player2.g > 1 || player2.g < 0)
                        {
                            var exitingPoint = (player2.g < 0) ? player2.boundTo.p1 : player2.boundTo.p2;
                            var nextList = geometry.LinesAttachedTo(exitingPoint).Where(l => l != player2.boundTo);
                            nextList = nextList.Where(l => (((((Vector2)player2.boundTo).CrossProduct(l) < 0) != player2.onCW) != (player2.boundTo.p1 == exitingPoint)) != ((exitingPoint == l.p1) == (exitingPoint == player2.boundTo.p1)));
                            if (nextList.Count() > 0)
                            {
                                var next = nextList.OrderBy(l => Math.Abs(((Vector2)player2.boundTo).CrossProduct(l)) / ((Vector2)l).Length()).Last();
                                var multiplier = Vector2.Dot(next, player2.boundTo) / (((Vector2)next).Length() * ((Vector2)player2.boundTo).Length()) * ((Vector2)player2.boundTo).Length() / ((Vector2)next).Length();
                                if ((exitingPoint == next.p1) == (exitingPoint == player2.boundTo.p1))
                                {
                                    player2.onCW = !player2.onCW;
                                }
                                player2.boundTo = next;
                                player2.g = (next.p1 == exitingPoint) ? 0 : 1;
                                player2.gv *= multiplier;
                                player2.ga = Vector2.Dot(player.gravity, (Vector2)next) / (((Vector2)next).LengthSquared());
                            }
                            else
                            {
                                isBound = false;
                                player.position = player2.boundTo.p1 + Vector2.Multiply(player2.boundTo, (float)player2.g);
                                player.velocity = Vector2.Multiply(player2.boundTo, (float)player2.gv);
                            }
                        }
                    }
                }
                else
                {
                    // order of this matters
                    player.position += player.velocity;
                    player.velocity += player.gravity;
                }
            }
        }

        internal void UpdatePerspective(GraphicsDevice GraphicsDevice, Matrix projectionMatrix, Matrix worldMatrix)
        {
            mouseListener.UpdatePerspective(GraphicsDevice, projectionMatrix, worldMatrix);
        }
    }
}

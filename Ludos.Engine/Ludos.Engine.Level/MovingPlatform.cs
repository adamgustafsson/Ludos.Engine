﻿namespace Ludos.Engine.Level
{
    using FuncWorks.XNA.XTiled;
    using Ludos.Engine.Actors;
    using Microsoft.Xna.Framework;
    using PointF = System.Drawing.PointF;
    using RectangleF = System.Drawing.RectangleF;

    public class MovingPlatform
    {
        private readonly Polyline _path;
        private int _currentLine;
        private int _direction;
        private Vector2 _position;
        private PointF _change;
        private RectangleF _platform;
        private RectangleF _detectionBounds;
        private float _defaultSpeed = 75;
        private float _speed;

        public MovingPlatform(Polyline polylinePath, Point size, float speedPct = 1)
        {
            _path = polylinePath;
            _direction = 1;
            _currentLine = 0;
            _change = new PointF(0, 0);
            _position = new Vector2(polylinePath.Bounds.X, polylinePath.Bounds.Y);
            _platform = new RectangleF(_position.X, _position.Y, size.X, size.Y);
            _speed = _defaultSpeed * speedPct;
        }

        public RectangleF DetectionBounds { get => _detectionBounds; }
        public RectangleF Bounds { get => _platform; }
        public PointF Change { get => _change; }
        public Actor Passenger { get; set; }

        public void Update(float a_elapsedTime)
        {
            if (Vector2.Distance(_path.Lines[_currentLine].Start, _position) > _path.Lines[_currentLine].Length)
            {
                if (_currentLine + 1 < _path.Lines.Length)
                {
                    _currentLine += 1;
                    _position = _path.Lines[_currentLine].Start;
                }
                else
                {
                    _direction = -1;
                    _position = _path.Lines[_currentLine].End;
                }
            }
            else if (Vector2.Distance(_position, _path.Lines[_currentLine].End) > _path.Lines[_currentLine].Length)
            {
                if (_currentLine - 1 >= 0)
                {
                    _currentLine -= 1;
                    _position = _path.Lines[_currentLine].End;
                }
                else
                {
                    _direction = 1;
                    _position = _path.Lines[_currentLine].Start;
                }
            }

            float targetPct = (_path.Lines[_currentLine].Length - Vector2.Distance(_position, _path.Lines[_currentLine].End)
                                + ((a_elapsedTime * _speed) * _direction)) / _path.Lines[_currentLine].Length;

            _position = Vector2.Lerp(_path.Lines[_currentLine].Start, _path.Lines[_currentLine].End, targetPct);
            _change.X = _position.X - _platform.X;
            _change.Y = _position.Y - _platform.Y;

            _platform.X = _position.X;
            _platform.Y = _position.Y;

            _detectionBounds = new RectangleF(_platform.X, _platform.Y, _platform.Width, _platform.Height * 0.20f);

            if (Passenger != null)
            {
                Passenger.Position = new Vector2(Passenger.Position.X + _change.X, _platform.Y - Passenger.Bounds.Height);
            }
        }
    }
}

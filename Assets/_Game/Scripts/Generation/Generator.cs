using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace Tofunaut.TofuECS_Rogue.Generation
{
    public class Config
    {
        public int FloorWidth;
        public int FloorHeight;
        public int MinSectionWidth;
        public int MaxSectionWidth;
        public int MinSectionHeight;
        public int MaxSectionHeight;
        public int TargetSectionCount;
        public int Seed;
    }

    public class Result
    {
        public int[] Tiles;
    }
    
    public class Generator
    {
        public int NumSections => _sections.Count;
        
        private readonly Config _config;
        private readonly Random _r;
        private readonly List<Section> _sections;
        private readonly int[] _tiles;

        public Generator(Config config)
        {
            _config = config;
            _tiles = new int[_config.FloorWidth * _config.FloorHeight];
            _sections = new List<Section>(_config.TargetSectionCount);
            _r = new Random(_config.Seed);
            
            _sections.Add(new Section
            {
                MinX = 0,
                MaxX = _config.FloorWidth,
                MinY = 0,
                MaxY = _config.FloorHeight
            });
        }

        public bool TrySplitSections()
        {
            var newSections = new List<Section>(_sections.Count);
            //_sections.Sort((a, b) => a.Area.CompareTo(b.Area));
            foreach (var currentSection in _sections)
            {
                if (_sections.Count + newSections.Count >= _config.TargetSectionCount)
                    break;

                var width = (int)(_r.NextDouble() * (currentSection.Width - _config.MinSectionWidth * 2));
                if (width <= 0)
                    continue;
                
                var height = (int)(_r.NextDouble() * (currentSection.Height - _config.MinSectionWidth * 2));
                if (height <= 0)
                    continue;
                    
                var allCorners = (Section.Corner[])Enum.GetValues(typeof(Section.Corner));
                var corner = allCorners[(int)Math.Floor(_r.NextDouble() * allCorners.Length)];
                currentSection.Insert(width, height, corner, out var insertedSection, out var secondarySection);
                newSections.Add(insertedSection);
                newSections.Add(secondarySection);
                //if (currentSection.Width <= _config.MinSectionWidth * 2)
                //{
                //    if (currentSection.Width <= _config.MinSectionWidth)
                //        continue;
                //    
                //    currentSection.SplitY(width + currentSection.MinX, out var newSection);
                //    newSections.Add(newSection);
                //}
                //else if (currentSection.Height <= _config.MinSectionHeight * 2)
                //{
                //    if (currentSection.Height <= _config.MinSectionHeight)
                //        continue;
                //    
                //    currentSection.SplitX(width + currentSection.MinX, out var newSection);
                //    newSections.Add(newSection);
                //}
                //else
                //{
                //}
            }

            if (newSections.Count <= 0)
                return false;

            foreach (var newSection in newSections)
                _sections.Add(newSection);

            return true;
        }

        public Result GetResult()
        {
            for (var i = 0; i < _sections.Count; i++)
            {
                for (var x = _sections[i].MinX; x < _sections[i].MaxX; x++)
                {
                    for (var y = _sections[i].MinY; y < _sections[i].MaxY; y++)
                    {
                        var index = y * _config.FloorWidth + x;
                        _tiles[index] = i + 1;
                    }
                }
            }

            return new Result
            {
                Tiles = _tiles,
            };
        }
    }
}
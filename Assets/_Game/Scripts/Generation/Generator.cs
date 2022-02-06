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
            _sections.Sort((a, b) => a.Area.CompareTo(b.Area));
            foreach (var currentSection in _sections)
            {
                if (_sections.Count + newSections.Count >= _config.TargetSectionCount)
                    break;

                var allCorners = (Section.Corner[])Enum.GetValues(typeof(Section.Corner));
                var corner = allCorners[(int)Math.Floor(_r.NextDouble() * allCorners.Length)];
                var width = Math.Min((int)(_r.NextDouble() * (_config.MaxSectionWidth - _config.MinSectionWidth)) +
                            _config.MinSectionWidth, currentSection.Width);
                var height = Math.Min((int)(_r.NextDouble() * (_config.MaxSectionHeight - _config.MinSectionHeight)) +
                             _config.MinSectionHeight, currentSection.Height);
                
                currentSection.Insert(width, height, corner, out var insertedSection, out var secondarySection);
                
                newSections.Add(insertedSection);
                newSections.Add(secondarySection);
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

        private void ShuffleSections(List<Section> sections)
        {
            for (var i = 0; i < sections.Count; i++)
            {
                var randIndex = (int)_r.NextDouble() * sections.Count;
                (sections[i], sections[randIndex]) = (sections[randIndex], sections[i]);
            }
        }
    }
}
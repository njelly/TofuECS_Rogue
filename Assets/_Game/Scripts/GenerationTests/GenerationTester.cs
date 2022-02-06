using System;
using TMPro;
using Tofunaut.TofuECS_Rogue.Generation;
using UnityEngine;
using UnityEngine.UI;

namespace Tofunaut.TofuECS_Rogue.GenerationTests
{
    public class GenerationTester : MonoBehaviour
    {
        [Header("Config")] 
        [SerializeField] private Vector2Int _floorSize;
        [SerializeField] private Vector2Int _minSectionSize;
        [SerializeField] private Vector2Int _maxSectionSize;
        [SerializeField] private int _seed;
        [SerializeField] private int _targetSectionCount;
        
        [Header("UI")]
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _resetButton;
        [SerializeField] private TextMeshProUGUI _numSectionsLabel;

        [Header("Temp")] 
        [SerializeField] private Gradient _sectionGradient;

        private Generator _generator;
        private SpriteRenderer _spriteRenderer;
        private Texture2D _texture;

        private void Awake()
        {
            _nextButton.onClick.RemoveAllListeners();
            _nextButton.onClick.AddListener(Next);
            _resetButton.onClick.RemoveAllListeners();
            _resetButton.onClick.AddListener(Reset);

            _texture = new Texture2D(_floorSize.x, _floorSize.y)
            {
                filterMode = FilterMode.Point,
            };
            
            var go = new GameObject("SpriteRenderer", typeof(SpriteRenderer));
            _spriteRenderer = go.GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = Sprite.Create(_texture, new Rect(Vector2.zero, Vector2.one * _floorSize), Vector2.one * 0.5f);

            Reset();
        }

        private void Update()
        {
            _numSectionsLabel.text = $"NumSections: {_generator.NumSections}";
        }

        private void Next()
        {
            if (!_generator.TrySplitSections())
                return;
            
            ApplyTexture();
        }

        private void ApplyTexture()
        {
            var result = _generator.GetResult();
            for (var i = 0; i < result.Tiles.Length; i++)
            {
                var color = _sectionGradient.Evaluate(result.Tiles[i] / (float)_targetSectionCount);
                var x = i % _floorSize.x;
                var y = i / _floorSize.y;
                _texture.SetPixel(x, y, color);
            }
            
            _texture.Apply();
        }

        private void Reset()
        {
            _generator = new Generator(new Config
            {
                FloorWidth = _floorSize.x,
                FloorHeight = _floorSize.y,
                MinSectionWidth = _minSectionSize.x,
                MinSectionHeight = _minSectionSize.y,
                MaxSectionWidth = _maxSectionSize.x,
                MaxSectionHeight = _maxSectionSize.y,
                Seed = _seed,
                TargetSectionCount = _targetSectionCount,
            });
            ApplyTexture();
        }

        private void OnValidate()
        {
            _minSectionSize.x = Math.Max(0, Math.Min(_minSectionSize.x, _maxSectionSize.x));
            _minSectionSize.y = Math.Max(0, Math.Min(_minSectionSize.y, _maxSectionSize.y));
        }
    }
}
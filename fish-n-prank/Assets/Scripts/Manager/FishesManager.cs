using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;


[CreateAssetMenu(menuName = "Managers/Fishes Manager")]
public class FishesManager : ScriptableObject
{

    [FancyHeader("Fishes Manager  ", 1.5f, "blue", 5.5f, order = 0)]
    [BoxGroup("List of fishes")] public List<FishSO> m_fishes;

    public SelectedFish GetRandomFish(List<AvailabilityArea> _areas = null, List<FishingRod> _rodes = null, Offshore _offshore = null, bool _flyingBird = false, bool _hasCorals = false)
    {
        List<FishSO> _fishes = m_fishes;
        // Shuffle
        _fishes.Sort((a, b)=> 1 - 2 * Random.Range(0, 1)); // "Hasard" Param in GDD managed here.

        FishSO _fish = _fishes.FirstOrDefault(f =>
            
            // Availability Area
            f.m_species.availability_area.Intersect(_areas).Any() &&
            // Rodes types
            f.m_species.fishing_rod.Intersect(_rodes).Any() &&
            // Offshore
            (f.m_species.offshore.min <= _offshore.min &&  f.m_species.offshore.max >= _offshore.max) &&
            // Coral fish
            (f.m_species.isCoralFish == _hasCorals) &&
            // A bird !
            (!_flyingBird)
            
            ); // Add more logic here...
        // Get a random size 
        List<FishSize> _fishSizesList = new List<FishSize>()
        {
            _fish.m_sizes.baby,
            _fish.m_sizes.little,
            _fish.m_sizes.normal,
            _fish.m_sizes.big,
            _fish.m_sizes.giant
        };
        
        FishSize _fishSize =  _fishSizesList[Random.Range(0, _fishSizesList.Count)];

        SelectedFish _selectedFish = new SelectedFish(){
            name = _fish.m_species.name.ToString(),
            size = _fishSize,
            weight = (_fish.m_species.base_weight * _fishSize.ratio),
            m_battleDuration = _fish.m_battleDuration,
            m_speed = _fish.m_speed
        };


        return _selectedFish;
    }

}

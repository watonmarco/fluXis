using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Map;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Mods;

public class RandomMod : IMod, IApplicableToMap
{
    public string Name => "Random";
    public string Acronym => "RD";
    public string Description => "Swaps the notes randomly.";
    public IconUsage Icon => Phosphor.Bold.ShuffleAngular;
    public ModType Type => ModType.Misc;
    public float ScoreMultiplier => 1f;
    public bool Rankable => false;
    public bool SaveScore => true;
    public Type[] IncompatibleMods => new[] { typeof(MirrorMod) };

    public void Apply(MapInfo map)
    {
        if (map.HitObjects.Count == 0)
            return;

        var keyCount = map.RealmEntry?.KeyCount ?? map.HitObjects.MaxBy(x => x.Lane)?.Lane ?? 0;

        if (keyCount <= 1)
            return;

        var switches = map.GetMapEvents()?.LaneSwitchEvents.OrderBy(e => e.Time).ToList() ?? new List<LaneSwitchEvent>();

        // para que las replays vean con la misma mezcla
        var seed = (map.Hash ?? map.FileName ?? Guid.NewGuid().ToString()).GetHashCode();
        var sectionIndex = 0;

        // secciones [start, nextStart), cada una con su propio set de lanes visibles
        var boundaries = new List<(double start, int count)> { (double.MinValue, keyCount) };

        foreach (var sw in switches)
            boundaries.Add((sw.Time, Math.Clamp(sw.Count, 1, keyCount)));

        boundaries.Add((double.MaxValue, -1));

        for (var i = 0; i < boundaries.Count - 1; i++)
        {
            var (start, count) = boundaries[i];
            var end = boundaries[i + 1].start;

            List<int> visibles;

            if (count >= keyCount)
                visibles = Enumerable.Range(1, keyCount).ToList();
            else
            {
                var row = LaneSwitchEvent.GetRow(count, keyCount, map.NewLaneSwitchLayout);
                visibles = row.Select((visible, idx) => (visible, lane: idx + 1))
                              .Where(x => x.visible)
                              .Select(x => x.lane)
                              .ToList();
            }

            if (visibles.Count <= 1)
            {
                sectionIndex++;
                continue;
            }

            var shuffled = visibles.ToList();
            var rng = new Random(seed + sectionIndex * 7919);

            for (var j = shuffled.Count - 1; j > 0; j--)
            {
                var k = rng.Next(j + 1);
                (shuffled[j], shuffled[k]) = (shuffled[k], shuffled[j]);
            }

            var mapping = visibles.Zip(shuffled).ToDictionary(x => x.First, x => x.Second);

            foreach (var hit in map.HitObjects)
            {
                if (hit.Time < start || hit.Time >= end)
                    continue;

                // las notas en lanes ocultas rompen todo. mejor se dejan como estan
                if (mapping.TryGetValue(hit.Lane, out var shuffledLane))
                    hit.Lane = shuffledLane;
            }

            sectionIndex++;
        }
    }
}

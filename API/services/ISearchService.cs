using API.Models;
using Microsoft.Extensions.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace API.services
{
    public interface ISearchService
    {
        RootViewModel searchItem(string SearchQuery);
    }

    public class SearchContext { }

    public class RootSearchContext : SearchContext
    {
        public IDictionary<string, int> BuildingMultiplyers { get; } = new Dictionary<string, int>();
        public IDictionary<string, int> GroupMultiplyers { get; } = new Dictionary<string, int>();

        public RootViewModel Root { get; set; }
    }

    public class ObjectSearch : ISearchService
    {
        private readonly RootViewModel _root;

        private readonly RootSearchContext _context;
        private readonly BuildingSearch _buildingSearch;
        private readonly LockSearch _lockSearch;
        private readonly GroupSearch _groupSearch;
        private readonly MediaSearch _mediaSearch;

        public ObjectSearch(RootViewModel root) // TODO: inject searchers?!
        {
            _root = root;

            _context = new RootSearchContext { Root = root };
            _buildingSearch = new();
            _lockSearch = new();
            _groupSearch = new();
            _mediaSearch = new();
        }

        public RootViewModel searchItem(string query)
        {
            foreach (var building in _root.buildings)
                _buildingSearch.Search(_context, building, query);
            foreach (var @lock in _root.locks)
                _lockSearch.Search(_context, @lock, query);
            foreach (var group in _root.groups)
                _groupSearch.Search(_context, group, query);
            foreach (var media in _root.media)
                _mediaSearch.Search(_context, media, query);

            return new()
            {
                buildings = _root.buildings.Where(b => b.weight > 0).ToList(),
                locks = _root.locks.Where(l => l.weight > 0).ToList(),
                groups = _root.groups.Where(g => g.weight > 0).ToList(),
                media = _root.media.Where(m => m.weight > 0).ToList(),
            };
        }
    }

    public interface IObjectSearch<TContext, T>
        where TContext : SearchContext
        where T : class
    {
        void Search(TContext context, T obj, string query);
    }

    public abstract class BaseSearch
    {
        public enum MatchType
        {
            None = 0,
            Partial = 1,
            Exact = 10
        }

        protected MatchType GetMatchType(string value, string query)
        {
            if (string.IsNullOrEmpty(value))
                return MatchType.None;
            if (value.Equals(query, System.StringComparison.OrdinalIgnoreCase))
                return MatchType.Exact;
            if (value.Contains(query, System.StringComparison.OrdinalIgnoreCase))
                return MatchType.Partial;
            return MatchType.None;
        }

        protected int SimpleWeight(string value, string query, int multiplyer, MatchType @override = MatchType.None)
        {
            var match = GetMatchType(value, query);
            if (match != MatchType.None && @override != MatchType.None)
                match = @override;

            return (int)match * multiplyer;
        }

        protected MatchType GetHighestMatchType(string query, params string[] values)
        {
            return values.Max(v => GetMatchType(v, query));
        }
    }

    public class BuildingSearch : BaseSearch, IObjectSearch<RootSearchContext, BuildingsViewModel>
    {
        private const int NameWeight = 9;
        private const int ShortcutWeight = 7;
        private const int DescriptionWeight = 5;

        public void Search(RootSearchContext context, BuildingsViewModel obj, string query)
        {
            var highestMatch = GetHighestMatchType(query, obj.name, obj.shortCut, obj.description);
            var nameMatch = GetMatchType(obj.name, query);
            if (nameMatch != MatchType.None)
            {
                context.BuildingMultiplyers[obj.id] = (int)nameMatch;
                obj.weight += NameWeight * (int)nameMatch;
            }

            obj.weight += SimpleWeight(obj.shortCut, query, ShortcutWeight, highestMatch);
            obj.weight += SimpleWeight(obj.description, query, DescriptionWeight, highestMatch);
        }
    }

    // needs to be evaluated after buildings
    public class LockSearch : BaseSearch, IObjectSearch<RootSearchContext, LocksViewModel>
    {
        private const int BuildingWeight = 8;

        private const int NameWeight = 10;
        private const int TypeWeight = 3;
        private const int SerialNumberWeight = 8;
        private const int FloorWeight = 6;
        private const int RoomNumberWeight = 6;
        private const int DescriptionWeight = 6;

        public void Search(RootSearchContext context, LocksViewModel obj, string query)
        {
            if (context.BuildingMultiplyers.TryGetValue(obj.buildingId, out var buildingMultiplyer))
                obj.weight += BuildingWeight * buildingMultiplyer;

            obj.weight += SimpleWeight(obj.name, query, NameWeight);
            obj.weight += SimpleWeight(obj.type, query, TypeWeight);
            obj.weight += SimpleWeight(obj.serialNumber, query, SerialNumberWeight);
            obj.weight += SimpleWeight(obj.floor, query, FloorWeight);
            obj.weight += SimpleWeight(obj.roomNumber, query, RoomNumberWeight);
            obj.weight += SimpleWeight(obj.description, query, DescriptionWeight);
        }
    }

    public class GroupSearch : BaseSearch, IObjectSearch<RootSearchContext, GroupsViewModel>
    {
        private const int NameWeight = 9;
        private const int DescriptionWeight = 5;

        public void Search(RootSearchContext context, GroupsViewModel obj, string query)
        {
            var nameMatch = GetMatchType(obj.name, query);
            if (nameMatch != MatchType.None)
            {
                context.GroupMultiplyers[obj.id] = (int)nameMatch;
                obj.weight += NameWeight * (int)nameMatch;
            }

            obj.weight += SimpleWeight(obj.description, query, DescriptionWeight);
        }
    }

    // needs to be evaluated after groups
    public class MediaSearch : BaseSearch, IObjectSearch<RootSearchContext, MediaViewModel>
    {
        private const int GroupWeight = 8;

        private const int TypeWeight = 3;
        private const int OwnerWeight = 10;
        private const int SerialNumberWeight = 8;
        private const int DescriptionWeight = 6;

        public void Search(RootSearchContext context, MediaViewModel obj, string query)
        {
            if (context.GroupMultiplyers.TryGetValue(obj.groupId, out var groupMultiplyer))
                obj.weight += GroupWeight * groupMultiplyer;

            obj.weight += SimpleWeight(obj.type, query, TypeWeight);
            obj.weight += SimpleWeight(obj.owner, query, OwnerWeight);
            obj.weight += SimpleWeight(obj.serialNumber, query, SerialNumberWeight);
            obj.weight += SimpleWeight(obj.description, query, DescriptionWeight);
        }
    }
}
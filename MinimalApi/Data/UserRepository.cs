namespace Data
{
    public record User(int Id, Guid ReferenceId, string Username, string Password, string DisplayName);

    class UserRepository
    {
        private readonly Dictionary<int, User> _usersById = new();
        private readonly Dictionary<Guid, User> _usersByReferenceId = new();

        public void Create(User user)
        {
            if (user is null)
            {
                return;
            }
            _usersById[user.Id] = user;
            _usersByReferenceId[user.ReferenceId] = user;
        }

        public User GetById(int id)
        {
            return _usersById.GetValueOrDefault(id);
        }

        public User GetByReferenceId(Guid referenceId)
        {
            return _usersByReferenceId.GetValueOrDefault(referenceId);
        }

        public List<User> GetAll()
        {
            return _usersById.Values.ToList();
        }

        public void Update(User user)
        {
            if (_usersById.ContainsKey(user.Id))
            {
                _usersById[user.Id] = user;
                _usersByReferenceId[user.ReferenceId] = user;
            }
        }

        public void Delete(int id)
        {
            if (_usersById.ContainsKey(id))
            {
                var user = _usersById[id];
                _usersById.Remove(id);
                _usersByReferenceId.Remove(user.ReferenceId);
            }
        }

        public void DeleteByReferenceId(Guid referenceId)
        {
            if (_usersByReferenceId.ContainsKey(referenceId))
            {
                var user = _usersByReferenceId[referenceId];
                _usersById.Remove(user.Id);
                _usersByReferenceId.Remove(referenceId);
            }
        }
    }
}

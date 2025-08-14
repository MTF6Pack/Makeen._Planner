using Domain;
using Domain.RepositoryInterfaces;
using Persistence.Repository.Base;

namespace Persistence.Repository;
public class GroupRepository(DataBaseContext context) : Repository<Group>(context), IGroupRepository;

using Denifia.Stardew.SendItems.Services;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Denifia.Stardew.SendItems.Domain
{
    public sealed class Repository
    {
        private static Repository instance = null;
        // adding locking object
        private static readonly object syncRoot = new object();
        private Repository() { }

        public static Repository Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new Repository();
                        }
                    }
                }
                return instance;
            }
        }

        private IConfigurationService _configService;

        public void Init(IConfigurationService configService)
        {
            _configService = configService;
        }       

        /// <summary>
        /// Insert a new document into collection. Document Id must be a new value in collection - Returns document Id
        /// </summary>
        public BsonValue Insert<T>(T entity, string collectionName = null)
        {
            using (var db = new LiteRepository(_configService.ConnectionString))
            {
                return db.Insert<T>(entity, collectionName);
            }
        }

        /// <summary>
        /// Insert an array of new documents into collection. Document Id must be a new value in collection. Can be set buffer size to commit at each N documents
        /// </summary>
        public int Insert<T>(IEnumerable<T> entities, string collectionName = null)
        {
            using (var db = new LiteRepository(_configService.ConnectionString))
            {
                return db.Insert<T>(entities, collectionName);
            }
        }

        /// <summary>
        /// Update a document into collection. Returns false if not found document in collection
        /// </summary>
        public bool Update<T>(T entity, string collectionName = null)
        {
            using (var db = new LiteRepository(_configService.ConnectionString))
            {
                return db.Update<T>(entity, collectionName);
            }
        }

        /// <summary>
        /// Update all documents
        /// </summary>
        public int Update<T>(IEnumerable<T> entities, string collectionName = null)
        {
            using (var db = new LiteRepository(_configService.ConnectionString))
            {
                return db.Update<T>(entities, collectionName);
            }
        }

        /// <summary>
        /// Insert or Update a document based on _id key. Returns true if insert entity or false if update entity
        /// </summary>
        public bool Upsert<T>(T entity, string collectionName = null)
        {
            using (var db = new LiteRepository(_configService.ConnectionString))
            {
                return db.Upsert<T>(entity, collectionName);
            }
        }

        /// <summary>
        /// Insert or Update all documents based on _id key. Returns entity count that was inserted
        /// </summary>
        public int Upsert<T>(IEnumerable<T> entities, string collectionName = null)
        {
            using (var db = new LiteRepository(_configService.ConnectionString))
            {
                return db.Upsert<T>(entities, collectionName);
            }
        }

        /// <summary>
        /// Delete entity based on _id key
        /// </summary>
        public bool Delete<T>(BsonValue id, string collectionName = null)
        {
            using (var db = new LiteRepository(_configService.ConnectionString))
            {
                return db.Delete<T>(id, collectionName);
            }
        }

        /// <summary>
        /// Delete entity based on Query
        /// </summary>
        public int Delete<T>(Query query, string collectionName = null)
        {
            using (var db = new LiteRepository(_configService.ConnectionString))
            {
                return db.Delete<T>(query, collectionName);
            }
        }

        /// <summary>
        /// Delete entity based on predicate filter expression
        /// </summary>
        public int Delete<T>(Expression<Func<T, bool>> predicate, string collectionName = null)
        {
            using (var db = new LiteRepository(_configService.ConnectionString))
            {
                return db.Delete<T>(predicate, collectionName);
            }
        }

        /// <summary>
        /// Returns new instance of LiteQueryable that provides all method to query any entity inside collection. Use fluent API to apply filter/includes an than run any execute command, like ToList() or First()
        /// </summary>
        public LiteQueryable<T> Query<T>(string collectionName = null)
        {
            using (var db = new LiteRepository(_configService.ConnectionString))
            {
                return db.Query<T>(collectionName);
            }
        }

        /// <summary>
        /// Search for a single instance of T by Id. Shortcut from Query.SingleById
        /// </summary>
        public T SingleById<T>(BsonValue id, string collectionName = null)
        {
            using (var db = new LiteRepository(_configService.ConnectionString))
            {
                return db.SingleById<T>(id, collectionName);
            }
        }

        /// <summary>
        /// Execute Query[T].Where(query).ToList();
        /// </summary>
        public List<T> Fetch<T>(Query query = null, string collectionName = null)
        {
            using (var db = new LiteRepository(_configService.ConnectionString))
            {
                return db.Fetch<T>(query, collectionName);
            }
        }

        /// <summary>
        /// Execute Query[T].Where(query).ToList();
        /// </summary>
        public List<T> Fetch<T>(Expression<Func<T, bool>> predicate, string collectionName = null)
        {
            using (var db = new LiteRepository(_configService.ConnectionString))
            {
                return db.Fetch<T>(predicate, collectionName);
            }
        }

        /// <summary>
        /// Execute Query[T].Where(query).First();
        /// </summary>
        public T First<T>(Query query = null, string collectionName = null)
        {
            using (var db = new LiteRepository(_configService.ConnectionString))
            {
                return db.First<T>(query, collectionName);
            }
        }

        /// <summary>
        /// Execute Query[T].Where(query).First();
        /// </summary>
        public T First<T>(Expression<Func<T, bool>> predicate, string collectionName = null)
        {
            using (var db = new LiteRepository(_configService.ConnectionString))
            {
                return db.First<T>(predicate, collectionName);
            }
        }

        /// <summary>
        /// Execute Query[T].Where(query).FirstOrDefault();
        /// </summary>
        public T FirstOrDefault<T>(Query query = null, string collectionName = null)
        {
            using (var db = new LiteRepository(_configService.ConnectionString))
            {
                return db.FirstOrDefault<T>(query, collectionName);
            }
        }

        /// <summary>
        /// Execute Query[T].Where(query).FirstOrDefault();
        /// </summary>
        public T FirstOrDefault<T>(Expression<Func<T, bool>> predicate, string collectionName = null)
        {
            using (var db = new LiteRepository(_configService.ConnectionString))
            {
                return db.FirstOrDefault<T>(predicate, collectionName);
            }
        }

        /// <summary>
        /// Execute Query[T].Where(query).Single();
        /// </summary>
        public T Single<T>(Query query = null, string collectionName = null)
        {
            using (var db = new LiteRepository(_configService.ConnectionString))
            {
                return db.Single<T>(query, collectionName);
            }
        }

        /// <summary>
        /// Execute Query[T].Where(query).Single();
        /// </summary>
        public T Single<T>(Expression<Func<T, bool>> predicate, string collectionName = null)
        {
            using (var db = new LiteRepository(_configService.ConnectionString))
            {
                return db.Single<T>(predicate, collectionName);
            }
        }

        /// <summary>
        /// Execute Query[T].Where(query).SingleOrDefault();
        /// </summary>
        public T SingleOrDefault<T>(Query query = null, string collectionName = null)
        {
            using (var db = new LiteRepository(_configService.ConnectionString))
            {
                return db.SingleOrDefault<T>(query, collectionName);
            }
        }

        /// <summary>
        /// Execute Query[T].Where(query).SingleOrDefault();
        /// </summary>
        public T SingleOrDefault<T>(Expression<Func<T, bool>> predicate, string collectionName = null)
        {
            using (var db = new LiteRepository(_configService.ConnectionString))
            {
                return db.SingleOrDefault<T>(predicate, collectionName);
            }
        }

    }
}

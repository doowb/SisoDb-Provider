using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SisoDb
{
	public interface ISingleOperationSession 
	{
	    int Count<T>() where T : class;
	    int Count<T>(Expression<Func<T, bool>> expression) where T : class;
	    bool Exists<T>(object id) where T : class;
	    T GetById<T>(object id) where T : class;
	    TOut GetByIdAs<TContract, TOut>(object id) where TContract : class where TOut : class;
	    string GetByIdAsJson<T>(object id) where T : class;
	    IList<T> GetByIds<T>(params object[] ids) where T : class;
	    IList<TOut> GetByIdsAs<TContract, TOut>(params object[] ids) where TContract : class where TOut : class;
	    IList<string> GetByIdsAsJson<T>(params object[] ids) where T : class;
	    IList<T> GetByIdInterval<T>(object idFrom, object idTo) where T : class;
	    IList<T> NamedQuery<T>(INamedQuery query) where T : class;
	    IList<TOut> NamedQueryAs<TContract, TOut>(INamedQuery query) where TContract : class where TOut : class;
	    IList<string> NamedQueryAsJson<T>(INamedQuery query) where T : class;
	    IList<T> RawQuery<T>(IRawQuery query) where T : class;
	    IList<TOut> RawQueryAs<TContract, TOut>(IRawQuery query) where TContract : class where TOut : class;
	    IList<string> RawQueryAsJson<T>(IRawQuery query) where T : class;
	    ISisoQueryable<T> Query<T>() where T : class;
	    T Insert<T>(T item) where T : class;
	    void InsertJson<T>(string json) where T : class;
	    void InsertMany<T>(IEnumerable<T> items) where T : class;
	    void InsertManyJson<T>(IEnumerable<string> json) where T : class;
	    T Update<T>(T item) where T : class;
	    void UpdateMany<T>(Expression<Func<T, bool>> expression, Action<T> modifier) where T : class;
	    void DeleteById<T>(object id) where T : class;
	    void DeleteByIds<T>(params object[] ids) where T : class;
	    void DeleteByIdInterval<T>(object idFrom, object idTo) where T : class;
	    void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class;
	}
}
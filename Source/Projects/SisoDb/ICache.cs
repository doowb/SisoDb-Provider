using System;
using System.Collections.Generic;
using SisoDb.PineCone.Structures;
using SisoDb.PineCone.Structures.Schemas;

namespace SisoDb
{
	/// <summary>
	/// A cache for a certain type of <see cref="IStructure"/> identified
	/// by <see cref="IStructureSchema.Name"/>.
	/// </summary>
	public interface ICache
	{
		/// <summary>
		/// The Structure Type that this cache manages.
		/// </summary>
		Type StructureType { get; }

		/// <summary>
		/// Clears all cache for the structure set.
		/// </summary>
		void Clear();

        /// <summary>
        /// Returns bool indicating if the Cache is empty or not.
        /// </summary>
        /// <returns></returns>
	    bool Any();

        /// <summary>
        /// Returns number indicationg how many items the cache holds.
        /// </summary>
        /// <returns></returns>
	    long Count();

        /// <summary>
        /// Returns bool indicating if there is a structure for
        /// the passed structure id in the cache.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
	    bool Exists(IStructureId id);

        /// <summary>
        /// Returns all items contained in the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
	    IEnumerable<T> GetAll<T>() where T : class;

        /// <summary>
		/// Returns either null or the structure matching the
		/// sent id.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		T GetById<T>(IStructureId id) where T : class;
		
		/// <summary>
		/// Returns a cached result set for all, none or subset of the passed ids.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ids"></param>
		/// <returns></returns>
		IDictionary<IStructureId, T> GetByIds<T>(IStructureId[] ids) where T : class;

		/// <summary>
		/// Called when an insert or update is performed against the <see cref="ISisoDatabase"/>.
		/// Should be used to update the cache.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <param name="structure"></param>
		/// <returns></returns>
		/// <remarks>Note! The structure being sent in should be returned by the cache.</remarks>
		T Put<T>(IStructureId id, T structure) where T : class;
		
		/// <summary>
		/// Called when an insert or update is performed against the <see cref="ISisoDatabase"/>.
		/// Should be used to update the cache.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items"></param>
		/// <returns></returns>
		/// <remarks>Note! The structures being sent in should be returned by the cache.</remarks>
		IEnumerable<T> Put<T>(IEnumerable<KeyValuePair<IStructureId, T>> items) where T : class;

		/// <summary>
		/// Called when a structure is being deleted from the <see cref="ISisoDatabase"/>.
		/// Should be used to update the cache.
		/// </summary>
		/// <param name="id"></param>
		void Remove(IStructureId id);

		/// <summary>
		/// Called when multiple structures are being deleted from the <see cref="ISisoDatabase"/>.
		/// Should be used to update the cache.
		/// </summary>
		/// <param name="ids"></param>
		void Remove(IEnumerable<IStructureId> ids);
	}
}
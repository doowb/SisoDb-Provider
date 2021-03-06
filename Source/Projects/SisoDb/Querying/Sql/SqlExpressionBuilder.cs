﻿using System;
using System.Linq;
using SisoDb.DbSchema;
using SisoDb.EnsureThat;
using SisoDb.NCore.Collections;
using SisoDb.NCore.Reflections;
using SisoDb.Querying.Lambdas;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Lambdas.Operators;

namespace SisoDb.Querying.Sql
{
    public class SqlExpressionBuilder : ISqlExpressionBuilder
    {
        protected readonly Func<ISqlWhereCriteriaBuilder> WhereCriteriaBuilderFn;

        public SqlExpressionBuilder(Func<ISqlWhereCriteriaBuilder> whereCriteriaBuilderFn)
        {
            Ensure.That(whereCriteriaBuilderFn, "whereCriteriaBuilderFn").IsNotNull();
            WhereCriteriaBuilderFn = whereCriteriaBuilderFn;
        }

        public virtual ISqlExpression Process(IQuery query)
        {
            Ensure.That(query, "query").IsNotNull();

            var expression = new SqlExpression();

            if(query.HasWhere)
                OnProcessWheres(query.Where, expression);

            if (query.HasSortings)
                OnProcessSortings(query.Sortings, expression);

            if (query.HasIncludes)
            {
                var mergedIncludeLambda = GetMergedIncludeLambda(query);
                OnProcessIncludes(mergedIncludeLambda, expression);
            }

            return expression;
        }

        protected virtual IParsedLambda GetMergedIncludeLambda(IQuery query)
        {
            if (!query.HasIncludes)
                return null;

            IParsedLambda mergedIncludes = null;
            foreach (var include in query.Includes)
            {
                mergedIncludes = mergedIncludes == null 
                    ? include 
                    : mergedIncludes.MergeAsNew(include);
            }

            return mergedIncludes;
        }

        protected virtual void OnProcessWheres(IParsedLambda lambda, SqlExpression expression)
        {
            var builder = WhereCriteriaBuilderFn();

        	for (var i = 0; i < lambda.Nodes.Length; i++)
        	{
        		var node = lambda.Nodes[i];
        	    if (node is MemberNode)
        	        OnProcessWhereMemberNode(lambda, expression, builder, i, (MemberNode) node);
        	    else if (node is OperatorNode)
        	        builder.AddOp((OperatorNode) node);
        	    else if (node is ValueNode)
        	        builder.AddValue((ValueNode) node);
        	    else if (node is NullNode)
        	        builder.AddNullValue((NullNode) node);
        	    else
        	        builder.AddRaw(node.ToString());

        	    builder.Flush();
        	}

            var whereCriteria = builder.IsEmpty
                ? SqlWhereCriteria.Empty()
                : new SqlWhereCriteria(builder.Sql, builder.Params.ToArray());

            expression.SetWhereCriteria(whereCriteria);
        }

        protected virtual void OnProcessWhereMemberNode(IParsedLambda lambda, SqlExpression expression, ISqlWhereCriteriaBuilder builder, int nodeIndex, MemberNode memberNode)
        {
            var memberIndex = expression.GetExistingOrNewMemberIndexFor(memberNode.Path);
            if (!expression.ContainsWhereMemberFor(memberNode.Path))
                expression.AddWhereMember(new SqlWhereMember(
                    memberIndex,
                    memberNode.Path,
                    memberNode.DataType,
                    memberNode.DataTypeCode));

            if (memberNode is InSetMemberNode)
            {
                OnProcessWhereInSetMemberNode(builder, (InSetMemberNode)memberNode, memberIndex);
                return;
            }

            if (memberNode is LikeMemberNode)
            {
                OnProcessWhereLikeMemberNode(builder, (LikeMemberNode)memberNode, memberIndex);
                return;
            }

            if (memberNode is StringContainsMemberNode)
            {
                OnProcessWhereStringContainsMemberNode(builder, (StringContainsMemberNode)memberNode, memberIndex);
                return;
            }

            if (memberNode is StringExactMemberNode)
            {
                OnProcessWhereStringExactMemberNode(builder, (StringExactMemberNode)memberNode, memberIndex);
                return;
            }

            if (memberNode is StringEndsWithMemberNode)
            {
                OnProcessWhereStringEndsWithMemberNode(builder, (StringEndsWithMemberNode)memberNode, memberIndex);
                return;
            }

            if(memberNode is StringStartsWithMemberNode)
            {
                OnProccessWhereStringStartsWithMemberNode(builder, (StringStartsWithMemberNode)memberNode, memberIndex);
                return;
            }

            if(memberNode is ToLowerMemberNode)
            {
                OnProccessWhereToLowerMemberNode(builder, (ToLowerMemberNode)memberNode, memberIndex);
                return;
            }

            if (memberNode is ToUpperMemberNode)
            {
                OnProccessWhereToUpperMemberNode(builder, (ToUpperMemberNode)memberNode, memberIndex);
                return;
            }

            if (memberNode.DataType.IsAnyBoolType())
            {
                var leftNode = lambda.Nodes.PeekLeft(nodeIndex);
                var rightNode = lambda.Nodes.PeekRight(nodeIndex);
                var isImplicitBoolNode = !(leftNode is OperatorNode) && !(rightNode is OperatorNode);

                if (isImplicitBoolNode)
                {
                    OnProcessWhereImplicitBoolMemberNode(builder, memberNode, memberIndex);
                    return;
                }
            }

            builder.AddMember(memberNode, memberIndex);
        }

        protected virtual void OnProcessWhereInSetMemberNode(ISqlWhereCriteriaBuilder builder, InSetMemberNode memberNode, int memberIndex)
        {
            builder.AddMember(memberNode, memberIndex);
            builder.AddOp(new OperatorNode(Operator.InSet()));
            builder.AddSetOfValues(new ArrayValueNode(memberNode.DataType, memberNode.DataTypeCode, memberNode.Values));
        }

        protected virtual void OnProcessWhereLikeMemberNode(ISqlWhereCriteriaBuilder builder, LikeMemberNode memberNode, int memberIndex)
        {
            builder.AddMember(memberNode, memberIndex);
            builder.AddOp(new OperatorNode(Operator.Like()));
            builder.AddValue(new ValueNode(memberNode.Value));
        }

        protected virtual void OnProcessWhereStringContainsMemberNode(ISqlWhereCriteriaBuilder builder, StringContainsMemberNode memberNode, int memberIndex)
        {
            builder.AddMember(memberNode, memberIndex);
            builder.AddOp(new OperatorNode(Operator.Like()));
            builder.AddValue(new ValueNode(string.Concat("%", memberNode.Value, "%").Replace("%%", "%")));
        }

        protected virtual void OnProcessWhereStringExactMemberNode(ISqlWhereCriteriaBuilder builder, StringExactMemberNode memberNode, int memberIndex)
        {
            builder.AddRaw("(");
            
            builder.AddMember(memberNode, memberIndex);
            builder.AddOp(new OperatorNode(Operator.Equal()));
            builder.AddValue(new ValueNode(memberNode.Value));
            builder.Flush();

            builder.AddRaw(" and ");

            const string castAsVarbinary = "cast({0} as varbinary(300))";
            builder.AddMember(memberNode, memberIndex, castAsVarbinary);
            builder.AddOp(new OperatorNode(Operator.Equal()));
            builder.AddLastValueAgain(castAsVarbinary);
            builder.Flush();

            builder.AddRaw(")");
        }

        protected virtual void OnProcessWhereStringEndsWithMemberNode(ISqlWhereCriteriaBuilder builder, StringEndsWithMemberNode memberNode, int memberIndex)
        {
            builder.AddMember(memberNode, memberIndex);
            builder.AddOp(new OperatorNode(Operator.Like()));
            builder.AddValue(new ValueNode(string.Concat("%", memberNode.Value)));
        }

        protected virtual void OnProccessWhereStringStartsWithMemberNode(ISqlWhereCriteriaBuilder builder, StringStartsWithMemberNode memberNode, int memberIndex)
        {
            builder.AddMember(memberNode, memberIndex);
            builder.AddOp(new OperatorNode(Operator.Like()));
            builder.AddValue(new ValueNode(string.Concat(memberNode.Value, "%")));
        }

        protected virtual void OnProccessWhereToLowerMemberNode(ISqlWhereCriteriaBuilder builder, ToLowerMemberNode memberNode, int memberIndex)
        {
            builder.AddMember(memberNode, memberIndex, "lower({0})");
        }

        protected virtual void OnProccessWhereToUpperMemberNode(ISqlWhereCriteriaBuilder builder, ToUpperMemberNode memberNode, int memberIndex)
        {
            builder.AddMember(memberNode, memberIndex, "upper({0})");
        }

        protected virtual void OnProcessWhereImplicitBoolMemberNode(ISqlWhereCriteriaBuilder builder, MemberNode memberNode, int memberIndex)
        {
            builder.AddMember(memberNode, memberIndex);
            builder.AddOp(new OperatorNode(Operator.Equal()));
            builder.AddValue(new ValueNode(true));
        }

        protected virtual void OnProcessSortings(IParsedLambda sortingsLambda, SqlExpression expression)
        {
            if (sortingsLambda == null || sortingsLambda.Nodes.Length == 0)
                return;

            foreach (var sortingNode in sortingsLambda.Nodes.OfType<SortingNode>())
            {
                var valueField = IndexStorageSchema.Fields.Value;

                if(expression.ContainsSortingMemberFor(sortingNode.MemberPath))
                    continue;

                var memberIndex = expression.GetExistingOrNewMemberIndexFor(sortingNode.MemberPath);

                expression.AddSortingMember(new SqlSortingMember(
                    memberIndex,
                    sortingNode.MemberPath,
                    string.Concat("mem", memberIndex),
                    valueField.Name,
                    sortingNode.Direction.ToString(), 
					sortingNode.DataType,
                    sortingNode.DataTypeCode));
            }
        }

        protected virtual void OnProcessIncludes(IParsedLambda includesLambda, SqlExpression expression)
        {
            if (includesLambda == null || includesLambda.Nodes.Length == 0)
                return;

            foreach (var includeNode in includesLambda.Nodes.OfType<IncludeNode>())
            {
                var nextNewIncludeIndex = expression.GetNextNewIncludeIndex();

                expression.AddInclude(new SqlInclude(
                    nextNewIncludeIndex,
                    includeNode.ReferencedStructureName,
                    string.Concat("inc", nextNewIncludeIndex),
                    IndexStorageSchema.Fields.Value.Name,
                    includeNode.IdReferencePath,
                    includeNode.ObjectReferencePath,
					includeNode.DataType,
                    includeNode.DataTypeCode));
            }
        }
    }
}
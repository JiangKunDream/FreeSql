﻿using FreeSql.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace FreeSql.SqlServer.Curd {

	class SqlServerDelete<T1> : Internal.CommonProvider.DeleteProvider<T1> where T1 : class {
		public SqlServerDelete(IFreeSql orm, CommonUtils commonUtils, CommonExpression commonExpression, object dywhere)
			: base(orm, commonUtils, commonExpression, dywhere) {
		}

		public override List<T1> ExecuteDeleted() {
			var sql = this.ToSql();
			if (string.IsNullOrEmpty(sql)) return new List<T1>();

			var sb = new StringBuilder();
			sb.Append(" OUTPUT ");
			var colidx = 0;
			foreach (var col in _table.Columns.Values) {
				if (colidx > 0) sb.Append(", ");
				sb.Append("DELETED.").Append(_commonUtils.QuoteSqlName(col.Attribute.Name)).Append(" as ").Append(_commonUtils.QuoteSqlName(col.CsName));
				++colidx;
			}

			var validx = sql.IndexOf(" WHERE ");
			if (validx == -1) throw new ArgumentException("找不到 WHERE ");
			sb.Insert(0, sql.Substring(0, validx));
			sb.Append(sql.Substring(validx));

			return _orm.Ado.Query<T1>(sb.ToString());
		}
	}
}
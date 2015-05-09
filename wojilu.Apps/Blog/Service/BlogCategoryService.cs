/*
 * Copyright (c) 2010, www.wojilu.com. All rights reserved.
 */

using System;
using System.Collections.Generic;

using wojilu.Apps.Blog.Domain;
using wojilu.Apps.Blog.Interface;
using wojilu.Common.AppBase;
using System.Data;

namespace wojilu.Apps.Blog.Service {

    public class BlogCategoryService : IBlogCategoryService {

        public virtual List<BlogCategory> GetByApp( int appId ) {
            return db.find<BlogCategory>( "AppId=" + appId + " order by OrderId desc, Id asc" ).list();
        }

        public virtual BlogCategory GetById( int id, int ownerId ) {
            BlogCategory result = db.findById<BlogCategory>( id );
            if (result.OwnerId != ownerId ) return null;
            return result;
        }

        public virtual void Insert( BlogCategory category ) {
            db.insert( category );
        }

        public virtual void Delete( BlogCategory category ) {

            db.updateBatch<BlogPost>( "SaveStatus=" + SaveStatus.Delete, "CategoryId=" + category.Id );
            db.delete( category );
        }

        public virtual void RefreshCache(BlogCategory category) {
        }

        public virtual List<BlogCategory> GetAll()
        {
            //return db.findBySql<BlogCategory>("select Name,count(*) as cnt from BlogCategory group by Name");//������Щ����sql��䵽����ôд
            //���淽��������select�б������ID����
            return db.find<BlogCategory>("1 = 1 order by Name , Id asc").list();

            //���ô���,��Ȼ������ִ��sql���,������foreach���鸳ֵ�ǻ�������
            //String sql = "select Name,count(*) as cnt from BlogCategory group by Name";
            //DataTable dt = db.RunTable<BlogCategory>(sql);
            //return dt;

        }

        public virtual List<BlogCategory> GetByIdName(int id)
        {
            BlogCategory blogCategory = db.findById<BlogCategory>(id);
            string sql = string.Format("Name = '{0}'", blogCategory.Name);

            return db.find<BlogCategory>(sql).list();
        }

    }
}


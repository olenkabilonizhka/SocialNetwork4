using DTO;
using ServiceStack;
using ServiceStack.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class PostRedisDAL
    {
        static string host = "127.0.0.1";
        static string port = "6379";
        static readonly IConnectionMultiplexer redis = ConnectionMultiplexer
            .Connect(new ConfigurationOptions { EndPoints = { host, port }, AbortOnConnectFail = false });
        static IDatabase db = redis.GetDatabase();
        static List<string> postIds = new List<string>();

        public static void CreatePost(PostRedis post)
        {
            if (!postIds.Contains(post.PostId))
                postIds.Add(post.PostId);

            HashEntry[] redisPost =
            {
                new HashEntry("userid",post.UserId),
                new HashEntry("title",post.Title),
                new HashEntry("body",post.Body),
                new HashEntry("likes",post.LikesPost.ToString()),
                new HashEntry("createdtime",post.CreatedTime.ToString("o"))
            };

            db.HashSet(post.PostId,redisPost);
            db.KeyExpire(post.PostId,new TimeSpan(0,0,100));
        }

        public static void DeletePost(string postId)
        {
            db.KeyDelete(postId);
            postIds.Remove(postId);
        }

        static PostRedis HashEntryToPostRedis(HashEntry[] hashE,string postId)
        {
            var post = new PostRedis { PostId = postId};
            foreach (var item in hashE)
            {
                switch(item.Name.ToString())
                {
                    case "userid":
                        post.UserId = item.Value.ToString();
                        break;
                    case "title":
                        post.Title = item.Value.ToString();
                        break;
                    case "body":
                        post.Body = item.Value.ToString();
                        break;
                    case "likes":
                        post.LikesPost = Convert.ToInt32(item.Value);
                        break;
                    case "crestedtime":
                        post.CreatedTime = Convert.ToDateTime(item.Value);
                        break;
                    default:
                        break;
                }
            }
            return post;
        }

        public static PostRedis GetPost(string postId)
        {
            var postR = db.HashGetAll(postId);
            var post = HashEntryToPostRedis(postR, postId);
            return post;
        }

        public static bool ifPostExists(string postId)
        {
            return (db.KeyExists(postId)) ? true : false;
        }

        public static bool ifPostsStreamExists()
        {
            if (postIds.Count == 0)
                return false;
            foreach (var item in postIds)
            {
                if (!ifPostExists(item))
                    return false;
            }
            return true;
        }

        public static List<PostRedis> GetAllPosts()
        {
            var list = new List<PostRedis>();
            foreach (var item in postIds)
            {
                list.Add(GetPost(item));
            }
            return list;
        }

        public static void LikePost(string postId, bool likes) //true - inc, false - dec
        {
            if (ifPostExists(postId))
            {
                db.HashIncrement(postId, "likes", (likes) ? 1 : -1);
            }
        }

        public static void UpdatePostBody(string postId, string newValue)
        {
            if (ifPostExists(postId))
            {
                db.HashSet(postId, "body", newValue);
            }
        }
    }
}

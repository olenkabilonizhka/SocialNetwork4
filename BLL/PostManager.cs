using AutoMapper;
using DAL;
using DTO;
using System.Collections.Generic;

namespace BLL
{
    public class PostManager
    {
        static IMapper mapper = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new PostProfile());
            mc.AddProfile(new CommentProfile());
        }).CreateMapper();

        public static List<Post> GetSortedPosts()
        {
            if (!PostRedisDAL.ifPostsStreamExists())
            {
                var list = PostDAL.GetSortedPosts();
                foreach (var item in list)
                    PostRedisDAL.CreatePost(mapper.Map<PostRedis>(item));
                return list;
            }
            else
                return mapper.Map<List<Post>>(PostRedisDAL.GetAllPosts());
        }

        public static Post GetPostById(string postId)
        {
            if (PostRedisDAL.ifPostExists(postId))
            {
                return mapper.Map<Post>(PostRedisDAL.GetPost(postId));
            }
            else
            {
                var post = PostDAL.GetPostById(postId);
                PostRedisDAL.CreatePost(mapper.Map<PostRedis>(post));
                return post;
            }
        }

        public static Comment GetCommentById(string postId, string commentId)
        {
            return PostDAL.GetCommentById(postId,commentId);
        }

        public static List<Post> GetUserPosts(string userId)
        {
            return PostDAL.GetUserPosts(userId);
        }

        public static string AddComment(string postId, string userId, Comment comment)
        {
            comment.Id = PostDAL.AddComment(postId, comment).Id;

            var commentD = mapper.Map<Comment,CommentDynamo>(comment);
            commentD.PostId = postId;
            commentD.UserId = userId;
            PostDynamoDAL.AddComment(commentD);

            return comment.Id;
        }

        public static string AddPost(Post post)
        {
            post.Id = PostDAL.AddPost(post).Id;

            var postD = mapper.Map<PostDynamo>(post);
            PostDynamoDAL.AddPost(postD);

            var postR = mapper.Map<PostRedis>(post);
            PostRedisDAL.CreatePost(postR);

            return post.Id;
        }

        public static void LikePost(string postId, string userIdCurrent)
        {
            bool likesIncDec = PostDAL.LikePost(postId,userIdCurrent);
            PostRedisDAL.LikePost(postId, likesIncDec);
        }

        public static void LikeComment(string postId, string commentId, string userIdCurrent)
        {
            PostDAL.LikeComment(commentId, userIdCurrent);
        }

        public static void SavePost(string postId, string newVal)
        {
            PostDynamoDAL.UpdatePost(postId, newVal);
            PostDAL.UpdatePost(postId,newVal);
            PostRedisDAL.UpdatePostBody(postId,newVal);
        }

        public static void SaveComment(string postId, string commentId, string newVal)
        {
            PostDynamoDAL.UpdateCommentBody(commentId,newVal);
            PostDAL.UpdateComment(postId,commentId,newVal);
        }

        public static List<Comment> GetCommentsSorted(string postId)
        {
            var list = new List<Comment>();
            foreach (var item in PostDynamoDAL.GetCommentsSortedByPostId(postId))
            {
                list.Add(GetCommentById(item.PostId, item.CommentId));
            }
            return list;
        }

        public static string GetUserIdByCommentId(string commentId)
        {
            var c = PostDynamoDAL.GetCommentById(commentId);
            return c.UserId;
        }

    }
}

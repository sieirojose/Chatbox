﻿using JoseSieiro_Chatbox_CalES.Models;
using JoseSieiro_Chatbox_CalES.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;

namespace JoseSieiro_Chatbox_CalES.Controllers
{
    public class ChatRoomController : Controller
    {
        private readonly ApplicationDbContext _context;
        // Constructor to initialize ApplicationDbContext
        public ChatRoomController(ApplicationDbContext context)
        {
            _context = context;
        }

		public IActionResult Index()
		{
			int? userId = HttpContext.Session.GetInt32("UserId");

			if (userId == null)
			{
				return RedirectToAction("Login", "Account");
			}

			var user = _context.Users.FirstOrDefault(u => u.Id == userId);
			var comments = _context.Comments
				.Where(comment => comment.UserId == userId)
				.Include(comment => comment.Replies)
					.ThenInclude(reply => reply.User)
				.Include(comment => comment.User)
				.OrderByDescending(comment => comment.CreatedOn)
				.ToList();

			var commentVm = new CommentVM
			{
				Comments = comments,
				FullName = user.FullName,
				Username = user.Username,
				Email = user.Email
			};

			return View(commentVm);
		}


		//Post: ChatRoom/PostReply
		[HttpPost]
        public async Task<IActionResult> PostReply(ReplyVM replyvm)
        {

            // Obtain the ID of the user of the HTTP session
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                //if the user is not present in the session, redirect him to the login view
                return RedirectToAction("Login", "Account");
            }

            var reply = new Reply
            {
                Text = replyvm.Reply,
                CommentId = replyvm.CommentId,
                CreatedOn = DateTime.Now,
                UserId = (int)userId
            };

            _context.Replies.Add(reply);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        //Post: ChatRoom/PostComment
        [HttpPost]
        public async Task<IActionResult> PostComment(string CommentText)
        {

            // Obtain the ID of the user of the HTTP session
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                //if the user is not present in the session, redirect him to the login view
                return RedirectToAction("Login", "Account");
            }

            var comment = new Comment
            {
                Text = CommentText,
                CreatedOn = DateTime.Now,
                UserId = (int)userId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

		//Post: ChatRoom/PostComment
		[HttpPost]
		public async Task<IActionResult> SearchComments(string filter)
		{


			// Obtain the ID of the user of the HTTP session
			int? userId = HttpContext.Session.GetInt32("UserId");

			if (userId == null)
			{
				//if the user is not present in the session, redirect him to the login view
				return RedirectToAction("Login", "Account");
			}

			var comments = await _context.Comments
	            .Where(comment => comment.UserId == userId)
				.Include(comment => comment.Replies)
					.ThenInclude(reply => reply.User)
				.Include(comment => comment.User)
				.OrderByDescending(comment => comment.CreatedOn)
	            .ToListAsync();

			var filteredComments = comments
		.Where(comment => string.IsNullOrEmpty(filter) ||
			(comment.Text != null && comment.Text.ToLower().Contains(filter.ToLower())))
		.OrderByDescending(comment => comment.CreatedOn)
		.ToList();

			// Crear una instancia de CommentVM y asignar los comentarios filtrados
			var commentVm = new CommentVM
			{
				Comments = filteredComments,
				// Asigna otras propiedades necesarias de CommentVM aquí
			};


			return PartialView("_Comments", commentVm);
		}



		////Get: /
		//[HttpGet]
		//public IActionResult Register()
		//{
		//    return View();
		//}


		////Post: /
		//[HttpPost]
		//public async Task<IActionResult> Register(RegisterVM register)
		//{

		//}

	}
}

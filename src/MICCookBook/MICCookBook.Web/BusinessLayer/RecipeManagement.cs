﻿using System.Data.Entity.Core;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using MICCookBook.Web.Models;
using MICCookBook.Web.Repository;
using MICCookBook.Web.Services;
using MICCookBook.Web.ViewModels;
using Microsoft.AspNet.Identity.Owin;

namespace MICCookBook.Web.BusinessLayer
{
    public class RecipeManagement : BaseManagement<RecipeManagement>
    {
        public RecipeManagement(IOwinContext owinContext) : base(owinContext)
        {
        }

        public async Task CreateNewRecipe(CreateRecipeViewModel model, IPrincipal user)
        {
            var unitOfWork = OwinContext.Get<UnitOfWork>();

            var fileStorageService = new LocalFileStorageService();
            var picturePath = fileStorageService.StoreFile(model.PictureFile);

            // create model
            var recipe = new Recipe()
            {
                Title = model.Title,
                Description = model.Description,
                Picture = picturePath,
                AuthorId = user.Identity.GetUserId()
            };

            // insert model in the database
            await unitOfWork.Recipes.Add(recipe);

            // save changes
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateRecipe(CreateRecipeViewModel model, IPrincipal user)
        {
            var unitOfWork = OwinContext.Get<UnitOfWork>();

            // retrieve model from database
            var recipe = await unitOfWork.Recipes.GetById(model.Id);
            if (recipe == null)
                throw new ObjectNotFoundException("The entry you are looking for doesn't exist in our database.");

            // update model
            recipe.Title = model.Title;
            recipe.Description = model.Description;
            if (model.PictureFile != null)
            {
                var fileStorageService = new LocalFileStorageService();
                var picturePath = fileStorageService.StoreFile(model.PictureFile);
                model.Picture = picturePath;
            }

            // save changes of modified model
            await unitOfWork.SaveChangesAsync();
        }
    }
}
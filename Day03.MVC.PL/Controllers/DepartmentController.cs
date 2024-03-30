﻿using Day03.BLL.Interfaces;
using Day03.BLL.Repositories;
using Day03.DAL.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Drawing;

namespace Day03.MVC.PL.Controllers
{
    //Inheritance: DepartmentController is a Controller
    //Composition: DepartmentController is a DepartmentRepositories
    public class DepartmentController : Controller
    {
        
		private readonly IUnitOfWork _unitOfWork;
		private readonly IHostEnvironment _env;

        public DepartmentController(IUnitOfWork unitOfWork, IWebHostEnvironment env)  //Ask Clr Create Object from "DepartmentRepositories"
        {
         
			_unitOfWork = unitOfWork;
			_env = env;
        }
        public IActionResult Index()
        {
			TempData.Keep();
			var departments = _unitOfWork.DepartmentRepository.GetAll();
            return View(departments);
        }

        public IActionResult Create()
        { 
            return View();
        }
         
        [HttpPost]
        public IActionResult Create(Department department)
        {
			
			if (ModelState.IsValid)   //Server Side Validation
            {
                 _unitOfWork.DepartmentRepository.Add(department);
                var count = _unitOfWork.Complete();
				//3.TempData
				if (count > 0)
					TempData["Message"] = "Department Is Created Successfully";
				else
					TempData["Message"] = "An Error ,Department Not Created";

				return RedirectToAction(nameof(Index));
			}
            return View(department);
        }


        // /Department/Details/10
        // /Department/Details
        [HttpGet]
        public IActionResult Details(int? id, string ViewName = "Details")
        {
            if (!id.HasValue)       // is null
                return BadRequest();    //Helper Method from ControllerBase   //400
            var department = _unitOfWork.DepartmentRepository.Get(id.Value);

            if (department is null)
                return NotFound();       //404

            return View(ViewName, department);
        }

		// /Department/Edit/10
		// /Department/Edit
		[HttpGet]
        public IActionResult Edit(int? id)
        {
            #region The same code in Details => Not Recomended
            ////if (!id.HasValue)       // is null
            ////    return BadRequest();    //Helper Method from ControllerBase   //400
            ////var department = _DepartmentsRepo.Get(id.Value);
            ////
            ////if(department is null)
            ////    return NotFound();    //404
            ////return View(department); 
            #endregion

            return Details(id, "Edit");   //Recomended  No Dublcate Code
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute]int id,Department department)
        {
            if(id!=department.Id)
                return BadRequest(" An Error ");
            if (!ModelState.IsValid)        //ControllerBase
            {
                return View(department);
            }

            try
            {
				_unitOfWork.DepartmentRepository.Update(department);
                _unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                //1.Log Exeption 
                //2.Friendly Massage
                if (_env.IsDevelopment())
                    ModelState.AddModelError(string.Empty, ex.Message);
                else
                    ModelState.AddModelError(string.Empty, "An Error During Update The Department");
                return View(department);    
            }
        }


        // /Department/Delete/10
        // /Department/Delete

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            return Details(id, "Delete");
        }

        [HttpPost]
        public IActionResult Delete(Department department)
        {
            try
            {
				_unitOfWork.DepartmentRepository.Delete(department);
                _unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (_env.IsDevelopment())
                    ModelState.AddModelError(string.Empty, ex.Message);
                else
                    ModelState.AddModelError(string.Empty, "An Error During Update The Department");
                return View(department);
            }
        }

    }
}

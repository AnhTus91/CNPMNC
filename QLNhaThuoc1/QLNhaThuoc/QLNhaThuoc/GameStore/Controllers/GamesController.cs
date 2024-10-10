using GameStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameStore.Controllers
{
    public class GamesController : Controller
    {
        QLThuocSo1VNEntities1 db = new QLThuocSo1VNEntities1();
        // GET: Games
        public ActionResult Index()
        {
            var listThuoc = from sp in db.SanPhams
                           select sp;
            var query = listThuoc.Include("HinhAnhs");
            return View(listThuoc.ToList());
        }

        public ActionResult DanhMuc(int? id)
        {   
            /* nếu không có mã danh mục sẽ báo lỗi*/
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            /* tìm kiếm sản phẩm theo mã danh mục*/
            var listThuoc = from sp in db.SanPhams
                               where sp.maDM == id
                               select sp;

            /* lấy hình ảnh ứng với sản phẩm*/
            var query = listThuoc.Include("DanhMuc").Include("HinhAnhs");

            /* Nếu không có sản phẩm nào sẽ báo lỗi*/
            if (query!=null)
                return View(listThuoc.ToList());

            return HttpNotFound();

         
            
           
        }

        public ActionResult ChiTietSP(int? id)
        {
           if(id== null) 
          return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var thuoc = from sp in db.SanPhams
                       where sp.maSP == id
                       select sp;
            var query = thuoc.Include("DanhMuc").Include("HinhAnhs");
                 
            return View(query.ToList());
        }


        public ActionResult Search(string keyword)
        {
            var listGame = from sp in db.SanPhams
                           select sp;
            var query = listGame.Include("HinhAnhs");

            if (!String.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(b => b.tenSP.ToLower().Contains(keyword));
            }

            return View(query.ToList());
        }
    }
}
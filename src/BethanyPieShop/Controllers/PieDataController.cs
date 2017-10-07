using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BethanyPieShop.Models;
using BethanyPieShop.ViewModels;
using Microsoft.AspNetCore.Http;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BethanyPieShop.Controllers
{
    [Route("api/[controller]")]
    public class PieDataController : Controller
    {
        private List<PieViewModel> m_Pies=new List<PieViewModel>();
        private int m_TotalPieCount;
        private readonly IPieRepository _pieRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PieDataController(IPieRepository pieRepository, IHttpContextAccessor httpContextAccessor)
        {
            _pieRepository = pieRepository;
            m_TotalPieCount = _pieRepository.Pies.Count();
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IEnumerable<PieViewModel> LoadMorePies(bool scrolling)
        {            
            int loadedPiesCount = _httpContextAccessor.HttpContext.Session.GetInt32("loadedPiesCount") ?? 0;
            if (!scrolling && (loadedPiesCount == m_TotalPieCount))
            {                
                loadedPiesCount = 0;
            }

            if (loadedPiesCount < m_TotalPieCount)
            {
                IEnumerable<Pie> dbPies = null;

                dbPies = _pieRepository.Pies.OrderBy(p => p.PieId).Skip(loadedPiesCount).Take(5);

                SetLoadedPiesCount(loadedPiesCount + dbPies.Count());

                foreach (var dbPie in dbPies)
                {
                    m_Pies.Add(MapDbPieToPieViewModel(dbPie));
                }
            }
            
            return m_Pies;
        }

        private void SetLoadedPiesCount(int pieCount)
        {
            _httpContextAccessor.HttpContext.Session.SetInt32("loadedPiesCount", pieCount);
        }

        private PieViewModel MapDbPieToPieViewModel(Pie dbPie)
        {
            return new PieViewModel()
            {
                PieId = dbPie.PieId,
                Name = dbPie.Name,
                Price = dbPie.Price,
                ShortDescription = dbPie.ShortDescription,
                ImageThumbnailUrl = dbPie.ImageThumbnailUrl
            };
        }
    }
}

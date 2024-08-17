namespace SubtitlesSiteMVCFrontend.Controllers;

using Microsoft.AspNetCore.Mvc;
using Moshi.SubtitlesSite.Models;
using Moshi.SubtitlesSite.Services;
using SubtitlesSiteMVCFrontend.Views.ViewModels;

public class SearchController : Controller
{
    private readonly MoviesService _moviesService;
    private readonly SubtitleService _subtitleService;
    private readonly UserService _userService;

    public SearchController(MoviesService moviesService, SubtitleService subtitleService, UserService userService)
    {
        _moviesService = moviesService;
        _subtitleService = subtitleService;
        _userService = userService;
    }

    // GET: Movies
    public IActionResult Index()
    {
        var movies = _moviesService.GetAllMovies();
        return View(movies);
    }

    // GET: Movies/Details/5
    public IActionResult Details(int id)
    {
        var movie = _moviesService.GetMovieById(id);
        if (movie == null)
        {
            return NotFound();
        }

        var comments = _subtitleService.GetCommentsWithUsernames(id);
        var commentViews = comments.Select(c => new SubtitleCommentView
        {
            Comment = c.Comment,
            UserName = c.Username,
            CommentDate = c.CommentDate
        });

        var viewModel = new MovieDetailsViewModel
        {
            Movie = movie,
            Subtitles = _subtitleService.GetSubtitlesByMovieId(id),
            AlternativeTitles = _moviesService.GetAlternativeTitles(id),
            MovieLinks = _moviesService.GetMovieLinks(id),
            Comments = commentViews
        };

        return View(viewModel);
    }

    // GET: Movies/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Movies/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Movie movie)
    {
        if (ModelState.IsValid)
        {
            var id = _moviesService.CreateMovie(movie);
            return RedirectToAction(nameof(Details), new { id });
        }
        return View(movie);
    }

    // GET: Movies/Edit/5
    public IActionResult Edit(int id)
    {
        var movie = _moviesService.GetMovieById(id);
        if (movie == null)
        {
            return NotFound();
        }
        return View(movie);
    }

    // POST: Movies/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Movie movie)
    {
        if (id != movie.MovieId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var success = _moviesService.UpdateMovie(movie);
            if (success)
            {
                return RedirectToAction(nameof(Details), new { id });
            }
        }
        return View(movie);
    }

    // GET: Movies/Delete/5
    public IActionResult Delete(int id)
    {
        var movie = _moviesService.GetMovieById(id);
        if (movie == null)
        {
            return NotFound();
        }
        return View(movie);
    }

    // POST: Movies/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var success = _moviesService.DeleteMovie(id);
        if (success)
        {
            return RedirectToAction(nameof(Index));
        }
        return NotFound();
    }

    // GET: Movies/Search
    public IActionResult Search(string query, char? letter)
    {
        IEnumerable<Movie> movies;
        if (!string.IsNullOrEmpty(query))
        {
            movies = _moviesService.SearchMovies(query);
        }
        else if (letter.HasValue)
        {
            movies = _moviesService.GetMoviesByFirstLetter(letter.Value);
        }
        else
        {
            movies = _moviesService.GetAllMovies();
        }

        var subtitleViewModels = new List<SubtitleViewModel>();
        foreach (var movie in movies)
        {
            var subtitles = _subtitleService.GetSubtitlesByMovieId(movie.MovieId);
            foreach (var subtitle in subtitles)
            {
                subtitleViewModels.Add(new SubtitleViewModel
                {
                    SubtitleId = subtitle.SubtitleId,
                    MovieTitle = movie.Title,
                    MovieYear = movie.Year,
                    SubtitleName = subtitle.OriginalFileName,
                    ReleaseInfo = subtitle.ReleaseInfo,
                    WatchOnlineLink = "#",
                    SubtitleSearcherLink = "#",
                    CdCount = subtitle.NumDiscs,
                    CommentCount = _subtitleService.GetCommentsCount(subtitle.SubtitleId),
                    UploadDate = subtitle.UploadDate,
                    Downloads = subtitle.Downloads,
                    SubtitleRating = _subtitleService.GetAverageRating(subtitle.SubtitleId),
                    MovieRating = movie.ImdbRating ?? 0,
                    UploaderName = _userService.GetUserById(subtitle.UserId).Username
                });
            }
        }

        ViewBag.CurrentLetter = letter;
        return View(subtitleViewModels);
    }

    // POST: Movies/AddAlternativeTitle/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddAlternativeTitle(int id, AlternativeTitle alternativeTitle)
    {
        if (ModelState.IsValid)
        {
            alternativeTitle.MovieId = id;
            var success = _moviesService.AddAlternativeTitle(alternativeTitle);
            if (success)
            {
                return RedirectToAction(nameof(Details), new { id });
            }
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: Movies/AddMovieLink/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddMovieLink(int id, MovieLink movieLink)
    {
        if (ModelState.IsValid)
        {
            movieLink.MovieId = id;
            var success = _moviesService.AddMovieLink(movieLink);
            if (success)
            {
                return RedirectToAction(nameof(Details), new { id });
            }
        }
        return RedirectToAction(nameof(Details), new { id });
    }
}
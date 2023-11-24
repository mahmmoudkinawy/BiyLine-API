namespace BiyLineApi.Features.Documents;
public sealed class GetDocumentByTypeFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string DocumentType { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<LegalDocumentEntity, Response>();
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IMapper _mapper;

        public Handler(BiyLineDbContext context, IMapper mapper)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var document = await _context.LegalDocuments
                .FirstOrDefaultAsync(ld =>
                    ld.Type == request.DocumentType.ToString(), cancellationToken: cancellationToken);

            if (document is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "Document with this key does not found"
                });
            }

            return Result<Response>.Success(_mapper.Map<Response>(document));
        }
    }
}

CREATE PROCEDURE DeleteUser
@UserId nvarchar(128)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE dbo.AspNetUsers
	SET IsDeleted = 1
	WHERE Id = @UserId
END
GO
/*
    Splits string into parts delimitered with specified character.
*/
IF	EXISTS	
(
	SELECT	* 
	FROM	dbo.SYSOBJECTS 
	WHERE	Id = OBJECT_ID(N'[dbo].[SDF_SplitString]')
	AND		type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' )
)
BEGIN
	DROP FUNCTION [dbo].[SDF_SplitString]
END
GO

CREATE FUNCTION [dbo].[SDF_SplitString]
(
    @sString nvarchar(2048),
    @cDelimiter nchar(1)
)
RETURNS @tParts TABLE ( part nvarchar(2048) )
AS
BEGIN
    if @sString is null return
    declare	@iStart int,
    		@iPos int
    if substring( @sString, 1, 1 ) = @cDelimiter 
    begin
    	set	@iStart = 2
    	insert into @tParts
    	values( null )
    end
    else 
    	set	@iStart = 1
    while 1=1
    begin
    	set	@iPos = charindex( @cDelimiter, @sString, @iStart )
    	if @iPos = 0
    		set	@iPos = len( @sString )+1
    	if @iPos - @iStart > 0			
    		insert into @tParts
    		values	( substring( @sString, @iStart, @iPos-@iStart ))
    	else
    		insert into @tParts
    		values( null )
    	set	@iStart = @iPos+1
    	if @iStart > len( @sString ) 
    		break
    end
    RETURN
END
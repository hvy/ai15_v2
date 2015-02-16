%Change this value to print another .mat file
load('polygObst.mat')

fid = fopen('output.txt','wt');
fprintf(fid,'%s\n','startPos');
fprintf(fid,'%s','x: ');
fprintf(fid,'%i\n',startPos(1, 1));
fprintf(fid,'%s','y: ');
fprintf(fid,'%i\n',startPos(1, 2));
fprintf(fid,'%s\n','goalPos');
fprintf(fid,'%s','x: ');
fprintf(fid,'%i\n',goalPos(1, 1));
fprintf(fid,'%s','y: ');
fprintf(fid,'%i\n',goalPos(1, 2));
fprintf(fid,'%s\n','New polygonal shape');
firstEdge=1;
for(i=1:length(x))

    edges(i,1)=i;
    if (button(i)==1)
        edges(i,2)=i+1;
    elseif (button(i)==3)
        edges(i,2)=firstEdge;
    end
    fprintf(fid,'%s','x');
    fprintf(fid,'%i',i - firstEdge);
    fprintf(fid,'%s',': ');
    fprintf(fid,'%f\n',x(edges(i,1)));
    fprintf(fid,'%s','y');
    fprintf(fid,'%i',i - firstEdge);
    fprintf(fid,'%s',': ');
    fprintf(fid,'%f\n',y(edges(i,1)));
    if (button(i)==3)
        firstEdge=i+1
        fprintf(fid,'%s\n','End of polygonal shape');
        if(i < length(x))
            fprintf(fid,'%s\n','New polygonal shape');
        end
    end

end

fclose(fid);
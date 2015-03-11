%Change this value to print another .mat file
clear;

load('../Assets/Levels/polygObst2.mat')

fid = fopen('output2.txt','wt');

for i=1:size(startPos, 1)
    fprintf(fid,'%s\n','New agent');
    fprintf(fid,'%s','x: ');
    fprintf(fid,'%d\n', startPos(i, 1));
    fprintf(fid,'%s','y: ');
    fprintf(fid,'%d\n', startPos(i, 2));
    fprintf(fid,'%s','x: ');
    fprintf(fid,'%d\n',goalPos(i, 1));
    fprintf(fid,'%s','y: ');
    fprintf(fid,'%d\n',goalPos(i, 2));
    fprintf(fid,'%s\n','End of agent');
end

fclose(fid);

% fprintf(fid,'%s\n','New polygonal shape');
% firstEdge=1;
% for(i=1:length(x))
% 
%     edges(i,1)=i;
%     if (button(i)==1)
%         edges(i,2)=i+1;
%     elseif (button(i)==3)
%         edges(i,2)=firstEdge;
%     end
%     fprintf(fid,'%s','x');
%     fprintf(fid,'%i',i - firstEdge);
%     fprintf(fid,'%s',': ');
%     fprintf(fid,'%f\n',x(edges(i,1)));
%     fprintf(fid,'%s','y');
%     fprintf(fid,'%i',i - firstEdge);
%     fprintf(fid,'%s',': ');
%     fprintf(fid,'%f\n',y(edges(i,1)));
%     if (button(i)==3)
%         firstEdge=i+1
%         fprintf(fid,'%s\n','End of polygonal shape');
%         if(i < length(x))
%             fprintf(fid,'%s\n','New polygonal shape');
%         end
%     end
% 
% end
% 
% for(i=1:size(customerPos, 1))
%     fprintf(fid,'%s\n','New customer');
%     fprintf(fid,'%s','x: ');
%     fprintf(fid,'%i\n',customerPos(i, 1));
%     fprintf(fid,'%s','y: ');
%     fprintf(fid,'%i\n',customerPos(i, 2));
%     fprintf(fid,'%s\n','End of customer');
% end
% 
% fclose(fid);
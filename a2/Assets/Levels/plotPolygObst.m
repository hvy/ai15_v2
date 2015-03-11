
%plot existing discrete obstacle map
load polygObst


hold on
firstEdge=1;
for(i=1:length(x))
    
    edges(i,1)=i;
    if (button(i)==1)
        edges(i,2)=i+1;
    elseif (button(i)==3)
        edges(i,2)=firstEdge;
        firstEdge=i+1
    end
    plot([x(edges(i,1)),x(edges(i,2))],[y(edges(i,1)),y(edges(i,2))]);
    hold on
    drawnow
end



plot(startPos(:,1),startPos(:,2),'*');
plot(goalPos(:,1),goalPos(:,2),'x');
plot(customerPos(:,1),customerPos(:,2),'o');

for i=1:length(startPos)
    plot([startPos(i,1) goalPos(i,1)],[startPos(i,2) goalPos(i,2)]);
end

hold off

%creating new maps
% figure
% plot([0 100 100 0 0],[0 0 100 100 0])
% 
% [x,y,button] = ginput();
% startPos=[30 70];
% goalPos=[80 30];

%save('polygObst','x','y','button','startPos','goalPos')

print -deps2 polygonalMap.eps













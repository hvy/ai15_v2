
%plot existing discrete obstacle map
load polygObst2


hold on

for i=1:12
    theta=(i-1)/12*pi;
    startPos(i,:)=[30*cos(theta) 30*sin(theta)];
    theta=theta+pi;
    goalPos(i,:)=[30*cos(theta) 30*sin(theta)];
end



plot(startPos(:,1),startPos(:,2),'*');
plot(goalPos(:,1),goalPos(:,2),'x');
%plot(customerPos(:,1),customerPos(:,2),'o');

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

save('polygObst2','startPos','goalPos')

print -deps2 polygonalMap2.eps












